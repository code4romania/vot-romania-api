using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotRomania.Models;
using VotRomania.Stores.Entities;

namespace VotRomania.Stores
{
    public class ApplicationContentRepository : IApplicationContentRepository
    {
        private readonly VotRomaniaContext _context;
        private readonly ILogger<ApplicationContentRepository> _logger;

        public ApplicationContentRepository(VotRomaniaContext context,
            ILogger<ApplicationContentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApplicationContentModel[]> GetApplicationContentAsync(Language? language = null)
        {
            return await _context.ApplicationContent
                .Where(x => language == null || x.Language == language.Value)
                .Select(x => x.ApplicationContent)
                .ToArrayAsync();
        }

        public async Task<(bool isSuccess, string errorMessage)> AddApplicationContentAsync(ApplicationContentModel content)
        {
            try
            {
                var languageContent = await _context.ApplicationContent.FirstOrDefaultAsync(x => x.Language == content.Language);
                if (languageContent != null)
                {
                    return (false, $"Content for language {content.Language} already exists");
                }

                var entity = new ApplicationContentEntity()
                {
                    Language = content.Language,
                    ApplicationContent = content
                };

                await _context.ApplicationContent.AddAsync(entity);

                return (true, string.Empty);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not add new polling station");
                return (false, e.Message);
            }
        }

        public async Task<(bool isSuccess, string errorMessage)> DeleteApplicationContentAsync(Language language)
        {
            try
            {
                var entity = await _context.ApplicationContent.FirstOrDefaultAsync(x => x.Language == language);
                if (entity == null)
                {
                    return (false, $"Could not find content for language = {language}");

                }

                _context.ApplicationContent.Remove(entity);
                _context.SaveChanges();

                return (true, string.Empty);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Could not delete content for language ={language}");
                return (false, e.Message);
            }
        }

        public async Task<(bool isSuccess, string errorMessage)> UpdateApplicationContentAsync(ApplicationContentModel content)
        {
            try
            {
                var entity = await _context.ApplicationContent.FirstOrDefaultAsync(x => x.Language == content.Language);
                if (entity == null)
                {
                    return (false, $"Could not find content for language = {content.Language}");

                }

                entity.ApplicationContent = content;


                _context.SaveChanges();

                return (true, string.Empty);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Could not update content for language {content.Language}");
                return (false, e.Message);
            }
        }
    }
}