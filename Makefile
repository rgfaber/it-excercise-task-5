.PHONY: test
test:
	dotnet test ProductDashboard.root.sln
 
.PHONY: run
run:
	dotnet run --project src/ProdDash.Rest 
 
