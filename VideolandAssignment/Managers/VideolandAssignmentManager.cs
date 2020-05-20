using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Rewrite.Internal.ApacheModRewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Remotion.Linq.Clauses.ResultOperators;
using VideolandAssignment.DTOModels;
using VideolandAssignment.Models;

namespace VideolandAssignment.Managers
{
    public class VideolandAssignmentManager
    {
        private const string LastUpdatedTimeStampName = "LastUpdated";
        // For demonstration purposes lower the maxShowId to be queried, otherwise it might take a long time to 
        private const int MaxShowId = 100;
        private const int PageSize = 10;

        public async void UpdateShows()
        {
            var updatedShowIds = await GetUpdatedShowIds();
            var lastUpdatedTime = await GetLastUpdatedTime();

            var toBeUpdated = updatedShowIds.Where(ts => ts.Value > lastUpdatedTime).Select(ts => ts.Key);
            // Getting all shows is quick enough to not need to be optimized to only get the new ones.
            var showDtos = await ShowsClient.GetAllShows();

            using (var context = new VideolandAssignmentContext())
            {
                List<Show> Shows = new List<Show>();
                long maxIdInStorage = 0;
                if(await context.Shows.FirstOrDefaultAsync() != null)
                {
                    maxIdInStorage = await context.Shows.MaxAsync(s => s.Id);
                }

                foreach (var dto in showDtos)
                {
                    // Only add shows not yet storage
                    if (dto.Id > maxIdInStorage)
                    {
                        Shows.Add(new Show()
                        {
                            Id = dto.Id,
                            Name = dto.Name
                        });
                    }
                }

                await context.Shows.AddRangeAsync(Shows);

                List<Person> peopleAlreadyInStorage = await context.Persons.ToListAsync();

                Dictionary<long, Person> personsDictionary = new Dictionary<long, Person>();

                Dictionary<Tuple<long, long>, ShowPerson> ShowPersonDict = new Dictionary<Tuple<long, long>, ShowPerson>();

                foreach (var toBeUpdatedCastShowId in toBeUpdated)
                {
                    if (toBeUpdatedCastShowId > MaxShowId)
                    {
                        break;
                    }
                    var castDtoList = await ShowsClient.GetCast(toBeUpdatedCastShowId);
                    var castPersonsList = castDtoList.Select(castDto => new Person()
                    {
                        Birthday = castDto.Person.Birthday,
                        Id = castDto.Person.Id,
                        Name = castDto.Person.Name
                    });
                    foreach (var person in castPersonsList)
                    {
                        ShowPersonDict[new Tuple<long, long>(toBeUpdatedCastShowId, person.Id)] = new ShowPerson()
                        {
                            PersonId = person.Id,
                            ShowId = toBeUpdatedCastShowId
                        };
                        // This will result with 1 entity per Person received from the api in the dictionary with the last one to be the one received from the api that ends up in the database, for the possibility that there might be an update to a person while querying
                        personsDictionary[person.Id] = person;
                    }

                }

                foreach(var person in personsDictionary.Values)
                {
                    InsertOrUpdatePerson(person, context);
                }

                foreach (var showPerson in ShowPersonDict.Values)
                {
                    InsertOrUpdateShowPerson(showPerson, context);
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<ShowListDto>> GetShowListByPage(int page = 0)
        {
            using (var context = new VideolandAssignmentContext())
            {
                var showListDtos = await context.Shows.Skip(page * PageSize).Take(PageSize).Select(s => new ShowListDto()
                {
                    Id = s.Id,
                    Name = s.Name

                }).ToListAsync();
                foreach (var showListDto in showListDtos)
                {
                    showListDto.Cast = await GetShowListPersonDtosForShowId(showListDto.Id, context);
                }

                return showListDtos;
            }
        }

        private async Task<List<ShowListPersonDto>> GetShowListPersonDtosForShowId(long id, VideolandAssignmentContext context)
        {
            var showPeople = await context.ShowPersons.Include(sp => sp.Person).Where(sp =>  sp.ShowId == id).OrderByDescending(sp => sp.Person.Birthday).ToListAsync();
            return showPeople.Select(p => new ShowListPersonDto()
            {
                Name = p.Person.Name,
                BirthDate = p.Person.Birthday?.ToString(" dd-MM-yyyy"),
                Id = p.Person.Id
            }).ToList();

        }

        private async void InsertOrUpdatePerson(Person person, VideolandAssignmentContext context)
        {
            if ( await context.Persons.FirstOrDefaultAsync(p => p.Id == person.Id) != null)
            {
                context.Persons.Update(person);
            }
            else
            {
                context.Persons.Add(person);
            }
        }

        private async void InsertOrUpdateShowPerson(ShowPerson showPerson, VideolandAssignmentContext context)
        {
            if (await context.ShowPersons.FirstOrDefaultAsync(sp => sp.PersonId == showPerson.PersonId && sp.ShowId == showPerson.ShowId) != null) {
                // nothing needs to happen as the keys are the only information the entity has and nothing can be updated
            }
            else
            {
                context.ShowPersons.Add(showPerson);
            }
        }

        private async Task<Dictionary<int, long>> GetUpdatedShowIds()
        {
            return await ShowsClient.GetUpdates();
        }

        public async void test()
        {
            using (var context = new VideolandAssignmentContext())
            {
            }
        }


        private async Task<bool> UpdateShow(ShowDTO showDto)
        {
            using (var context = new VideolandAssignmentContext())
            {

            }

            return true;
        }


        private async Task<long> GetLastUpdatedTime()
        {
            using (var context = new VideolandAssignmentContext())
            {
                var stamp = await context.UnixTimeStamps
                    .SingleOrDefaultAsync((ts => ts.Id == LastUpdatedTimeStampName));
                if(stamp != null)
                {
                    return stamp.TimeStamp;
                }
            }

            return 0;
        }
    }
}
