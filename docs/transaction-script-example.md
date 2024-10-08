

```
// Charge their credit card?
try {
var txId = await creditCardService.ChargeCardAsync('jeffg', 1299);
await sql.ExecuteAsync(@"
	BEGIN TRANSACTION
	UPDATE Products  SET (QTY = 18 ) where (ProductId=xyzpdq AND Price=1424.00 AND Qty=19)
	SELECT TotalPurchases as CurrentPurchases FROM Customers Where CustomerId=jeffg
	UPDATE Customers SET (TotalPurchases = TotalPurchases + 1424.00 ) where CustomerId=Jeffg # dirty reads? hmmm.
	INSERT Shipping (Product=xyzpdq, Customer=jeffg)
    COMMIT TRANSACTION
")
} catch (CreditCardException ex) {
	
	// uh, throw?
	throw ex;
} catch (SqlException ex) {
	await creditCardService.CancelTransaction(txId); // what if THIS throws?
	throw ex;
}
```