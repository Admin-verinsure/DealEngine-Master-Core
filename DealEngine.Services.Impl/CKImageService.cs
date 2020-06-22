using System;
using System.Linq;
using System.Net.Mime;
using System.IO;
using DealEngine.Services.Interfaces;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using NHibernate.Linq;


namespace DealEngine.Services.Impl
{
    public class CKImageService : ICKImageService
    {
        IMapperSession<CKImage> _ckimageRepository;

        public CKImageService(IMapperSession<CKImage> ckimageRepository)
        {
            _ckimageRepository = ckimageRepository;
        }

        #region ICKImageService implementation

        public async Task Upload(CKImage ckimage)
        {
            await _ckimageRepository.AddAsync(ckimage);
        }

        public async Task Delete(CKImage ckimage)
        {
            await _ckimageRepository.RemoveAsync(ckimage);
        }

        public Task Update(CKImage ckimageUpdate)
        {
            throw new NotImplementedException();
        }

        public async Task<CKImage> GetCKImage(string path)
        {
            return await _ckimageRepository.FindAll().FirstOrDefaultAsync(i => i.Path == path);
        }

        public Task<List<CKImage>> GetAllImages()
        {
            return _ckimageRepository.FindAll().ToListAsync();                
        }           
 
        #endregion


    }

}