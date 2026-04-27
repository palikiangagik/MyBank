using MyBank.Application.Interfaces;

namespace MyBank.Application.Helpers
{
    public class UnitOfWorkScope : IDisposable, IAsyncDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private bool _commited;

        private UnitOfWorkScope(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _commited = false;
        }

        public static async Task<UnitOfWorkScope> StartAsync(IUnitOfWork unitOfWork)
        {
            await unitOfWork.BeginTransactionAsync();
            return new UnitOfWorkScope(unitOfWork);
        }

        public async Task CommitAsync()
        {
            await _unitOfWork.CommitAsync();
            _commited = true;
        }

        public void Dispose()
        {
            if (!_commited)
                _unitOfWork.Rollback();
        }

        public async ValueTask DisposeAsync()
        {
            if (!_commited)
                await _unitOfWork.RollbackAsync();
        }

    }
}
