using System;
using System.Linq;
using System.Net.Mime;
using System.IO;
using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using NHibernate.Linq;


namespace TechCertain.Services.Impl
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

        public Task Delete(int Id)
        {
            throw new NotImplementedException();
        }
      
        public Task Update(CKImage ckimageUpdate)
        {
            throw new NotImplementedException();
        }

        public Task GetCKImage(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<List<CKImage>> GetAllImages()
        {
            return _ckimageRepository.FindAll().ToListAsync();                
        }           
 
        /*
        public async Task UploadFile(Image image)
        {
            _imageRepository.AddAsync(image);
        }

        public async Task<Document> GetDocument(string documentName)
        {
            return await _documentRepository.FindAll().FirstOrDefaultAsync(i => i.Name == documentName);
        }

        
        public byte [] ToBytes (string contents)
        {
            return System.Text.Encoding.UTF8.GetBytes (contents);
        }

        public string FromBytes (byte [] bytes)
        {
            return System.Text.Encoding.UTF8.GetString (bytes);
        }

         */
        #endregion


    }

}