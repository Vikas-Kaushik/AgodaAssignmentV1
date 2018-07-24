# AgodaAssignmentV1
Please refer ReadMeAlso.pdf for problem statement and solution approach.

Language: C#
Tools: Visual Studio 2007 Professional Version 15.7.4
Framework: .Net core 2.1

How to run:
1. Clone the repo from https://github.com/Vikas-Kaushik/AgodaAssignmentV1/
2. Configure max requests, slot time, time to block values in appsettings.json
3. Get postman, dotNet sdk 2.1 and Visual studio code installed on your machine.
4. Open the solution folder i.e. "AgodaAssignmentV1\code\" in Visual Studio Code
5. Hit F5 to run
6. Open "AgodaAssignmentV1\Hotels.API.Tests.postman_collection.json" in postman.
7. Run the collection

Just FYI:
1. File: "AgodaAssignmentV1\code\RateLimitService\RateLimitService.cs" has the algorithm implemented 
2. Use this to test it, File: "AgodaAssignmentV1\code\RateLimitServiceTest\RateLimitServiceShould.cs"
3. Enpoints exposed are: "/api/hotels/city/{cityName}" and "/api/hotels/room/{roomType}"

Here're more details:
[embed]https://raw.githubusercontent.com/vkaushik/AgodaAssignmentV1/master/ReadMeAlso.pdf[/embed]

![embed]https://raw.githubusercontent.com/vkaushik/AgodaAssignmentV1/master/ReadMeAlso.pdf[/embed]

![More Details PDF](https://raw.githubusercontent.com/vkaushik/AgodaAssignmentV1/master/ReadMeAlso.pdf)
