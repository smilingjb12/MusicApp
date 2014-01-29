using System.Collections.Generic;
using Data.Domain;

namespace Business.Services
{
    public interface ITagService
    {
        Tag GetOrCreateTag(string name);
        IEnumerable<Tag> GetOrCreateTags(IEnumerable<string> tagNames); 
    }
}
