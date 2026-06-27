# FlightDex - Flight Timetable and Ticket Booking Application

## Live URL: https://lively-ground-0f1850200.7.azurestaticapps.net/

Flightdex is Flight Timetable and Ticket Booking Application made with .NET 10 and Angular 22. It enables the user to view the timetable of flights operating from a selected Airport, search flights and book tickets. The project is designed in vertical slices.

The backend is a .NET 10 modular monolith with clean-onion architecture, CQRS and Outbox Pattern. The main API flight fetches paginated data for the timetable and enables searching and filtering flights. The two supporting APIs airports and ticket enable search suggestions and ticket booking. 

The frontend is built with Angular 22. It is a modern UI built with standalone components, signals-first reactivity and new control-flow syntax. The highlight being Timetable being a complete single-screen layout dashboard. 

The deployment is done with Azure Container Apps, Azure Static Web Apps, Key Vault, EntraID, Azure SQL and Azure Deployment Stacks. Deployment is done using Biceps IaC with entire fullstack provisioned in 'azd up'. 

# Demo - [Documentation/Demo.md](Documentation/Demo.md)
# Demo VIdeo - https://drive.google.com/drive/folders/1K2yO6RojVGDnnszDrxhhJw4jI5I4jPzB?usp=drive_link
> Kindly ignore the old demo video without voice and refer the Google Drive Link above.  

# Detailed Documentation:
- Design - [Documentation/Design.md](Documentation/Design.md)
- Strucucture - [Documentation/Structure.md](Documentation/Structure.md)
- Features - [Documentation/Features.md](Documentation/Features.md)
- Flow - [Documentation/Flow.md](Documentation/Flow.md) 
- Postmortem - [Documentation/Solutions/D32P1_Solution.md](Documentation/Solutions/D32P1_Solution.md)

# Piece Solutions:
- Solutions upto Day 29 in their respective brances:
    - Day 22 Piece 2 - https://github.com/thinkbridge-thinkschool/AryanB-Capstone-FlightDex/tree/D22P2_CapstoneKickoff_Design_Scaffold 
    - Day 23 Piece 1 - https://github.com/thinkbridge-thinkschool/AryanB-Capstone-FlightDex/tree/D23P1---Bicep-IaC
    - Day 24 Piece 1 - https://github.com/thinkbridge-thinkschool/AryanB-Capstone-FlightDex/tree/D24P1---DeploymentStacks_azd
    - Day 25 Piece 1 - https://github.com/thinkbridge-thinkschool/AryanB-Capstone-FlightDex/tree/D25P1---Identity_End-to-End
    - Day 26 Piece 1 - https://github.com/thinkbridge-thinkschool/AryanB-Capstone-FlightDex/tree/D26P1---AppInsights-KQL
    - Day 27 Piece 1 - https://github.com/thinkbridge-thinkschool/AryanB-Capstone-FlightDex/tree/D27P1---SecurityPass
    - Day 28 Piece 1 - https://github.com/thinkbridge-thinkschool/AryanB-Capstone-FlightDex/tree/D28P1---DesignReview_ADR
- Solutions from Day 29:
    - Day 29 Piece 1 - [Documentation/Solutions/D29P1_Solution.md](Documentation/Solutions/D29P1_Solution.md)
    - Day 30 Piece 1 - [Documentation/Solutions/D30P1_Solution.md](Documentation/Solutions/D30P1_Solution.md)
    - Day 31 Piece 1 - [Documentation/Solutions/D31P1_Solution.md](Documentation/Solutions/D31P1_Solution.md)
        - [![Integration, Perf and Security](https://github.com/thinkbridge-thinkschool/AryanB-Capstone-FlightDex/actions/workflows/ci.yml/badge.svg)](https://github.com/thinkbridge-thinkschool/AryanB-Capstone-FlightDex/actions/workflows/ci.yml)
    - Day 32 Piece 1 - [Documentation/Solutions/D32P1_Solution.md](Documentation/Solutions/D32P1_Solution.md)