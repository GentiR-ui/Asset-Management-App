namespace AssetManagementSystem.Domain.Common;

/// <summary>
/// Shenon entitetet qe mbajne fushat e auditit: CreatedAt, CreatedBy, UpdatedAt, UpdatedBy.
///
/// NUK deklaron veti me qellim. ApplyAuditFields shkruan permes entry.Property(emer),
/// qe e anashkalon aksesueshmerine e C#-it — pra setters-at e BaseEntity mbeten
/// 'protected' dhe asnje sherbim nuk mund t'i shkruaje me dore.
///
/// Ekziston sepse User trashegon IdentityUser&lt;Guid&gt; dhe C# nuk lejon trashegimi te
/// dyfishte: pa kete shenjues, tabela e userave do mbetej e vetmja pa audit.
///
/// ⚠️ Emrat e vetive duhet te qendrojne IDENTIKE te BaseEntity dhe te User — cikli i
/// auditit i kerkon me emer, jo me tip.
/// </summary>
public interface IAuditable;
