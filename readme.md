## Videoland Assignment project

At first I wanted to use a .NET Core framework like https://abp.io/ because it provides a nice default application structure with useful generic classes and nice dependency injection already set up and features like a job queue which could be used for the api scraping and is what I would use to start a new serious project.
But it seems it's using .Net Core 3.x and thus doesn't fit the spec. 
I tried https://aspnetboilerplate.com/ but it's .NET Core specific documentation was lacking for even the basic stuff this project needed.

# Database

This project uses a Sqlite Database that is auto-generated when the Update-Database migration is run.

The database updates when the /UpdateDb post call is received.

# Api

The project provides a swagger page on the server's root when running.

# TvMaze rate limiting

TvMaze limits its api by providing a maximum of 20 requests per 10 seconds, unless they have the result cached.
The caching seems fairly limited because I can't find a reliable method for a universally cached show's cast information, so I resorted to trying to minimize api calls.
My thinking was that there's more people than shows so it makes more sense to call the api for a show's cast than to call the api for a person's casted-on shows.
I also tried to simply embed the cast in the show's index, which if possible would provide all the information needed but that's not possible.
Normal embeds in a show call "/shows/1?embed=cast" are not well cached only would simply result in more waiting.

The api's documentation also says that you can keep your local show data up-to-date by calling the index with the page derived from your last show's id.
But if you need the cast data this doesn't work because with a running show, cast can get added.

The only thing you can do is use the updates call and store the resulting's timestamp. Then the next time you call updates you can filter out all the shows that haven't been updated since the last time.

# Demonstration

For demonstration purposes, I have hardcoded a maximum showId it will look up the cast information otherwise it might take an hour or more to call the whole TvMaze api. 
To get more cast simply increase that variable and call updateDb again, it should skip the ones which information it already has.