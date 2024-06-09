# Linken Fuzzy search

### Installation

Add package.

### Usage

```csharp
var Helper = new SearchHelper<Invoice>(Database.Invoices.Where(v => v.Type == 03).ToList());
Helper.GetSimpleSearch = (i) => i.InvoiceNumber;
Helper.ListChanged = () = >InvokeAsync(StateHasChanged);
Helper.StringSearch = "142";
```
