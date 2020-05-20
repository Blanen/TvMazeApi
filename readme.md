## Videoland Assignment project

At first I wanted to use a .NET Core framework like https://abp.io/ because it provides a nice default application structure and features like a job queue and is what I would use to start a new serious project.
But it seems it's using .Net Core 3.x and thus doesn't fit the spec.

# Database

This project uses a Sqlite Database that is auto-generated when the Update-Database migration is run.

# Api

The project provides a swagger page on the server's root when running.

# TvMaze rate limiting

TvMaze limits its api by providing a maximum of 20 requests per 10 seconds, unless they have the result cached.
The caching seems fairly limited because I can't find a reliable method, so I resorted to trying to minimize api calls.
My thinking was that there's more people than shows so it makes more sense to call the api for a show's cast than to call the api for a person's casted-on shows.
I also tried to simply embed the cast in the show's index, which if possible would provide all the information needed but that's not possible.

The api's documentation also says that you can keep your local show data up-to-date by calling the index with the page derived from your last show's id.
But if you need the cast data this doesn't work because with a running show, cast can get added.

The only thing you can do is use the updates call and store the resulting's timestamp. Then the next time you call updates you can filter out all the shows that haven't been updated since the last time.