# Day 32 — Ship + demo + postmortem

## Live URL: https://lively-ground-0f1850200.7.azurestaticapps.net/

## Demo Video Link: https://github.com/thinkbridge-thinkschool/AryanB-Capstone-FlightDex/blob/main/Documentation/Demo-Video.mp4

## Demo Doc: https://github.com/thinkbridge-thinkschool/AryanB-Capstone-FlightDex/blob/main/Documentation/Demo.md

## Postmortem:

### What I would do differently?

If I could plan the project from scratch again, I take a problem statement which has more user involvement. My problem statement initially involved just the timetable and a bookmark interface. This had very little user involvement. This was pointed out by interviewer Rushikesh K, after which I incorporated the Ticket Booking part in my project.
I would also think about the flows and backend components according the architecture of the project. The project was a modular monolith clean-onion architecture project and I used to think in terms of a simple Minimal API or the converntional API architecture. This would lead to problems finding use cases for utillizing evyry part of a more sophisticated architecture.

### What the hardest bug taught me?

During the initial stage while running the application through Docker, the application would return invalid credentials for users who registered in the last application build. Even though, the user credential would be correct. The users schema was not the problem as it was not dropping the data and the data existed there. Which made the problem more confusing.
The reason was password hashes are one-way, so the stale accounts couldn't be migrated — only reset.Stopped the container, dropped the user rows, restarted. New sign-ups then hash under the current scheme and log in fine. 
Thus, The hardest bug taught me sometimes problems can arrise from places outside code. It can be a infrastructure or filesystem issue. 

### What I am proudest about?
I am proudest about designing the timetable component as a one-screen layout dashboard UI. The layout keeps consistent across all operations, never expanding beyond the viewport. A one-screen UI is a UI design which fits on the screen's viewport and keeps all operation accessible within the viewport. 
I find it highly satisfying to use a one-screen layout UI, since it always has everything in sight. It looks clean and readible. In the project, without this layout loading the next page in departures and arrivals would shift the website up or down. Which can be annoying and might cause one to loose the track of what's he looking for. This layout successfully solves the issue, while further improving readability and user excperience.