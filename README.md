# IT Exercise: Task 5

## DESCRIPTION

This is an exercise project that:

- must be written in C#
- takes JSON data from a data source
- executes a number of queries on that data
- returns the results as JSON data
- must be disclosed using a REST API

## MOTIVATION

- As a **Candidate for the position of Backend Engineer**
- I must **Produce a service that fulfills the requirements above**
- In order to **Progress to the next phase of the selection process**

## OUTLINE

- [IT Exercise: Task 5](#it-exercise-task-5)
    - [DESCRIPTION](#description)
    - [MOTIVATION](#motivation)
    - [OUTLINE](#outline)
    - [BEHAVIOR](#behavior)
        - [Scenario 1: Retrieve JSON from URL](#scenario-1-retrieve-json-from-url)
    - [Architectural Choices](#architectural-choices)
        - [ProdDash.Prov](#proddashprov)
        - [ProdData.Client](#proddataclient)

## BEHAVIOR

This section describes a number of scenario

### Scenario 1: Retrieve JSON from URL

The application should retrieve product data from ANY url.

- GIVEN
    - Any URL that returns JSON list of product data
- WHEN
    - URL = https://flapotest.blob.core.windows.net/test/ProductData.json
- THEN
    - Read the data as JSON

## Architectural Choices

- We opt for a _Inverted Arhitecture_ style approach, with dependencies pointing inwards. This opens the possibility to
  switch implementations if so required.
- The application will have 3 packages:

```mono

----> = dependency
<~~~~ = data flow

         +----------------+     +---------------+     +-----------------+    
~~JSON~~>|  ProdDash.Rest |---->| ProdDash.Api  |<----| ProdData.Client |<~~JSON~~[URL] 
<~~JSON~~|    <<webapi>>  |     | <<classlib>>  |     |   <<classlib>>  |
         +----------------+     +---------------+     +-----------------+

```

### ProdDash.Api

Is a **classlib** that implements the main API for the exerc



### ProdData.Client

Is a **classlib** that has following responsibilities:

- implement the _IProdData_ interface which is defined in ProdDash.Prov


