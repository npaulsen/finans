# Hanz finans
Hanz Finans is a private household account book based on categorizing and analysing transactions.

There may be established [alternatives](#-alternatives), but I wanted to play around and use something between Excel and shiny finance apps.

In its current state, it only runs locally, esp. without any integrations.

## Features
### Importing CSV-formatted transaction data
 - Detecting delimiter and where the transaction headers start:
<img width="1152" alt="image" src="https://github.com/npaulsen/finans/assets/286686/43056a6f-9ea4-46d5-af47-417ef67c1608">
 - ...and column mapping, also with some guessing to minimize manual efforts:
   
<img width="764" alt="image" src="https://github.com/npaulsen/finans/assets/286686/62e2b771-0744-485b-9532-f34df7a2c5d8">

### Analysing Earnings and Spendings per month / quarter / any period
<img width="1421" alt="image" src="https://github.com/npaulsen/finans/assets/286686/41d347f0-d4cb-478c-a13a-4f6a2ae036fd">

### Defining custom rules and categories of spendings / earnings
(not yet via FE)

## Alternatives
Partial list in no particular order.
 - https://apps.apple.com/de/app/finanzblick-online-banking/id401912744
 - https://apps.apple.com/de/app/finanzguru-konten-verträge/id1214803607
 - https://apps.apple.com/de/app/outbank-banking-finances/id1094255754
 - https://apps.apple.com/de/app/moneystats-budget-planner/id1501244873
 - https://apps.apple.com/de/app/money-manager-moneycoach/id989642198
 - https://apps.apple.com/de/app/cashculator-personal-finance/id1591642644?l=en-GB&mt=12
 - https://apps.apple.com/de/app/microsoft-excel/id462058435 😉
 - ... and zillions more

## Quickstart  :snail:
The quickest way to get started is using a __VS Code devcontainer__ setup.

The devcontainer comes with the dotnet SDK and some preinstalled VS Code extensions. 

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [VS Code](https://code.visualstudio.com/download) with the extension [Remote - Containers](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers) (`ms-vscode-remote.remote-containers`)

### Get started
- When you open the repo in VS Code, the remote extension will suggest reopening the workspace in a container. Do it :)
- Start the server with `ctrl`+`F5`


## References
 - https://www.reddit.com/r/Finanzen/comments/pk75h4/ing_stellt_die_features_umsatzanalyse_und_budgets/
