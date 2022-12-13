.PHONY: default
default: help

.PHONY: tests
tests:
  dotnet test
 
.PHONY: run
run:
  dotnet run src/ProdDash.Rest 
 
