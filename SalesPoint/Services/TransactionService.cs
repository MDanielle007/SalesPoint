using AutoMapper;
using SalesPoint.DTO;
using SalesPoint.Exceptions;
using SalesPoint.Interfaces;
using SalesPoint.Models;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SalesPoint.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ITransactionProductRepository _transactionProductRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public TransactionService(
            ITransactionRepository transactionRepository,
            ITransactionProductRepository transactionProductRepository,
            IUserRepository userRepository,
            IProductRepository productRepository,
            IMapper mapper,
            ILogger logger)
        {
            _transactionRepository = transactionRepository;
            _transactionProductRepository = transactionProductRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TransactionDTO> CreateTransactionAsync(TransactionCreateDTO transactionDTO)
        {
            using var transactionScopes = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var user = await _userRepository.GetUserByIdAsync(transactionDTO.UserId);

                if (user == null)
                {
                    throw new NotFoundException($"User with ID {transactionDTO.UserId} not found");
                }

                if (transactionDTO.Products == null || transactionDTO.Products.Any())
                {
                    throw new BadRequestException("Transaction must contain at least one product");
                }

                var transactionProducts = new List<TransactionProduct>();
                decimal totalAmount = 0;

                foreach (var productDTO in transactionDTO.Products)
                {
                    var product = await _productRepository.GetProductByIdAsync(productDTO.ProductId);

                    if (product == null)
                    {
                        throw new NotFoundException($"Product with ID {productDTO.ProductId} not found");
                    }

                    if (product.Status != Enum.ProductStatus.Active)
                    {
                        throw new BadRequestException($"Product {product.Name} is not available for sale");
                    }

                    if (product.Quantity < productDTO.Quantity)
                    {
                        throw new BadRequestException($"Insufficient stock for product {product.Name}.\nAvailable: {product.Quantity}\nRequested: {productDTO.Quantity}");
                    }

                    var productTotal = productDTO.Quantity * product.SellingPrice;

                    var transactionProduct = new TransactionProduct
                    {
                        ProductId = productDTO.ProductId,
                        Quantity = productDTO.Quantity,
                        UnitPrice = product.SellingPrice,
                        TotalPrice = productTotal,
                    };

                    transactionProducts.Add(transactionProduct);
                    totalAmount += productTotal;

                    product.Quantity -= productDTO.Quantity;
                    await _productRepository.UpdateProductAsync(product);
                }

                if (transactionDTO.AmountPaid < totalAmount)
                {
                    throw new BadRequestException($"Insufficient payment. Total: {totalAmount}, Paid: {transactionDTO.AmountPaid}");
                }

                var transaction = new SalesPoint.Models.Transaction
                {
                    UserId = transactionDTO.UserId,
                    TotalAmount = totalAmount,
                    AmountPaid = transactionDTO.AmountPaid,
                    ChangeAmount = transactionDTO.AmountPaid - totalAmount,
                    Status = Enum.TransactionStatus.Completed,
                    DateTime = DateTime.Now,
                    Products = transactionProducts
                };

                var createdTransaction = await _transactionRepository.AddTransactionAsync(transaction);

                var result = _mapper.Map<TransactionDTO>(createdTransaction);

                result.UserName = $"{user.FirstName} {user.LastName}";
                result.Products = _mapper.Map<List<TransactionProductDTO>>(createdTransaction.Products);

                foreach ( var productDTO in result.Products )
                {
                    var product = createdTransaction.Products.First(p => p.ProductId == productDTO.ProductId).Product.Name;
                }

                transactionScopes.Complete();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction");
                throw;
            }
        }

        public async Task<TransactionDTO> GetTransactionByIdAsync(int transactionId)
        {
            try
            {
                var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId, includeDetails: true);

                if(transaction == null)
                {
                    throw new NotFoundException($"Transaction with id {transactionId} not found");
                }

                var result = _mapper.Map<TransactionDTO>(transaction);
                result.UserName = $"{transaction.User.FirstName} {transaction.User.LastName}";
                result.Products = _mapper.Map<List<TransactionProductDTO>>(transaction.Products);

                foreach (var productDTO in result.Products)
                {
                    var product = transaction.Products.First(p => p.ProductId == productDTO.ProductId).Product.Name;
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting transaction with id {transactionId}");
                throw;
            }
        }

        public async Task<IEnumerable<TransactionDTO>> GetTransactionsAsync(TransactionFilterDTO filter)
        {
            try
            {
                var transactions = await _transactionRepository.GetTransactionsAsync(filter, includeDetails: true);

                var result = new List<TransactionDTO>();

                foreach(var transaction in transactions)
                {
                    var transactionDTO = _mapper.Map<TransactionDTO>(transaction);
                    transactionDTO.UserName = $"{transaction.User.FirstName} {transaction.User.LastName}";
                    transactionDTO.Products = _mapper.Map<List<TransactionProductDTO>>(transaction.Products);

                    foreach (var productDTO in transactionDTO.Products)
                    {
                        var product = transaction.Products.First(p => p.ProductId == productDTO.ProductId).Product.Name;
                    }

                    result.Add(transactionDTO);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transactions");
                throw;
            }
        }

        public async Task CancelTransactionAsync(int transactionId)
        {
            using var transactionScopes = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);
                if(transaction == null)
                {
                    throw new NotFoundException($"Transaction with id {transactionId} not found");
                }

                if(transaction.Status == Enum.TransactionStatus.Cancelled)
                {
                    throw new BadRequestException($"Transaction with id {transactionId} is already cancelled");
                }
                
                foreach(var transactionProduct in transaction.Products)
                {
                    var product = await _productRepository.GetProductByIdAsync(transactionProduct.ProductId);

                    if(product != null)
                    {
                        product.Quantity += transactionProduct.Quantity; 
                        await _productRepository.UpdateProductAsync(product);
                    }
                }

                transaction.Status = Enum.TransactionStatus.Cancelled;
                await _transactionRepository.UpdateTransactionAsync(transaction);

                transactionScopes.Complete();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling transaction with id {transactionId}");
                throw;
            }
        }
    }
}
