using Account.Application.Common.Abstractions;
using Account.Domain.Entities;

namespace Account.Application.Contracts;

public interface IAuditLogRepository : IRepositoryWriter<AuditLog> { }