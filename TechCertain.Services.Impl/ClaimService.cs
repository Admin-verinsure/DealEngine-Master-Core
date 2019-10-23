using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;


namespace TechCertain.Services.Impl
{
    public class ClaimService : IClaimService
    {
        IMapperSession<AuthClaims> _AuthClaimsRepository;

        public ClaimService(IMapperSession<AuthClaims> clientAgreementRuleRepository, IMapperSession<ClientAgreement> clientAgreementRepository)
        {
            _AuthClaimsRepository = clientAgreementRuleRepository;
        }

        public Task AddClaimAsync(IdentityRole role, System.Security.Claims.Claim claim, CancellationToken cancellationToken = default)
        {
            if (cancellationToken != null)
                cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            //var roleClaimEntity = new AuthClaims
            //{
            //    Name = claim.Type,
            //    Description = claim.Value,
            //    RoleId = role.Id
            //};

            //_unitOfWork.RoleClaimRepository.Add(roleClaimEntity);
            //_unitOfWork.Commit();

            return Task.CompletedTask;
        }

        public Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public AuthClaims CreateClaims(User createdBy, string name, string description)
        {
            //if (_AuthClaimsRepository.FindAll().FirstOrDefault(r => r.Name == name) != null)
            //    return null;
            //if (string.IsNullOrWhiteSpace(name))
            //    throw new ArgumentNullException(nameof(name));
            //if (string.IsNullOrWhiteSpace(description))
            //    throw new ArgumentNullException(nameof(description));

            AuthClaims AuthClaim = new AuthClaims(createdBy, "", "");
            //_AuthClaimsRepository.AddAsync(AuthClaim);
            return AuthClaim;
           
        }

        public Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<IdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public AuthClaims[] GetAllClaims( )
        {
            IQueryable<AuthClaims> claims = _AuthClaimsRepository.FindAll();
           
            return claims.ToArray();
        }

        public Task<IList<System.Security.Claims.Claim>> GetClaimsAsync(IdentityRole role, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveClaimAsync(IdentityRole role, System.Security.Claims.Claim claim, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(IdentityRole role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetRoleNameAsync(IdentityRole role, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
