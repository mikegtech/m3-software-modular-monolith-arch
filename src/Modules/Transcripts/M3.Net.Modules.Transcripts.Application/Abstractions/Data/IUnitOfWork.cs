using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3.Net.Modules.Transcripts.Application.Abstractions.Data;
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    // Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
