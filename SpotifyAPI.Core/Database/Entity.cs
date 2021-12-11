using System;
using EntityFrameworkCore.Triggers;
using MassTransit;

namespace SpotifyAPI.Core.Database
{
    public abstract class Entity
    {
        static Entity()
        {
            Triggers<Entity>.GlobalUpdating.Add(entry => { entry.Entity.Updated = DateTime.UtcNow; });

            Triggers<Entity>.GlobalUpdating.Add(entry => { entry.Entity.Revision = NewId.NextGuid(); });

            Triggers<Entity>.GlobalInserting.Add(
                entry =>
                {
                    var time = DateTime.UtcNow;
                    entry.Entity.Created = time;
                    entry.Entity.Updated = time;
                }
            );

            Triggers<Entity>.GlobalInserting.Add(entry => { entry.Entity.Revision = NewId.NextGuid(); });
        }

        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Guid Revision { get; set; }
    }
}