# VR-Scraper

## Purpose
The projects purpose is scraping the web for different media (e.g. Manga Capters, TV-Show Episodes, ...).
There will be other microservices responsible for e.g. saving the scraped results. See other repositories with VR*.

## How to setup
You need at least Docker Engine Release 18.06.0 (due to docker-compose file format v3.7) and docker-compose installed.
Be sure to check out the .env file. The sensible parts of the service are configured via the environment. Normally the .env file would not be part of
the repository (I know), but as everything runs locally in Docker and is not accessible from outside, there is no problem with it. If you want to change the setup you should update the variables in the .env file accordingly.

1. Execute *docker-compose up* 
    * builds the images, spins up the containers, reads the .env file and starts the containers

For debugging without attaching to the container I also added launchSettings.json under /Properties. These would also normally not be included in the repository for security reasons but due to the already mentioned circumstances it it is ok.
You can run the ASP.NET Core API as the environment variables are simply pasted into the file. You will also need to run *dotnet tool restore* as we use playwright-sharp (https://github.com/microsoft/playwright-sharp) and want to avoid downloading the browsers during the runtime.
After that you have to call *dotnet playwright-sharp install-browsers* to download them.

## How to use
VR-Persistence exposes its API on localhost:${VRScraperApiPort}.
Under /resources there is a Postman-Collection with example requests which can be used to test the service. Just dont forget to update the port in accordance with ${VRPersistenceApiPort}.