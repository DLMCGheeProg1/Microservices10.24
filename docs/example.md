# Example

Let's say I am developing a service that allows employees to log incidents for supported software.

The *operation* my service allows is `EmployeeLoggedIncident`.

We *reference* data on the outside (cached data) especially if we don't "own" it.
We can *embed* data that we "own" and is immutable (doesn't have *versions*).

Our service cannot change any referenced data. We have no transactional consistency in that.
We can change our own data, because we *do* have transactional consistency around that.
What does our service *own*? Incidents. 
	- No other service can change incidents.

To perform the operation, I need the following *operands*.
They need to give me a *reference* to a piece of software that is supported.
	- the data is owned by some other service.
They need to give me a *reference* to their Employee ID.
	- the data is owned by some other service (Identity, OIDC, etc.)
They need to give ma description to what the incident is about.
	- I'm going to own this (my service), so I can just validate it.

What's my trust level?
Do I need to *validate* that reference to the software?
Do I need to *validate* that JWT that identifies the user?

Minimize danger of invalidated references:
	- The software
		- would it be a big deal if the reference was out of date? They removed that software, or the employee lost their entitlement to that software?
	- the Identity
		- Would it be a big deal if that token was expired or revoked?
		- A JWT has an expiration and the concept of "refresh tokens".

What is the volatility of the references? How often do they change? Are they mutable?
- a software catalog item is just a title and description.
	- Questions (real ones): 
	- Does the title or description ever change?
	- Would our processing of  the `EmployeeLoggedIncident` change if the title or the description changed?
	- Is it *ephemeral*? Does it's existence as a "thing" come and go?
		- People at your company that left decades ago have some remnant as a "reference" somewhere. (Right to Forget is a EU Hippy Nonsense!)
	- Could/Should we process an `EmployeeLoggedIncident` if by the time we get the incident, the catalog item is "gone"?
		- If "no" - do we send them a 404 or something? Pretend like it never happened.
		- Would there be *value* in tracking incidents for software not in the catalog?
			- We could decide how to process that *later*.
			- e.g. It's *still* a logged incident, but if you look at it, maybe we set the status to "`OnHoldSoftwareNotInCatalog`". Or whatever. (if we just *reject* it there is lost data that might be useful. E.g. "Hey, we keep getting incidents for this software that is out of the catalog, we might want to contact the support folks to tell them to uninstall that thing)

Why did we let users create *incidents* instead of *issues*? 

```
Operation: AddSignatoryToAccount
Operands:
	- ref:
		- Account Holder Identity
		- Version (timestamp?)
	- ref:
		- AccountId
		- Version
	- emb: 
		- Information about the the person you are adding.
```

If your service doesn't own the account information, can it really add this to the account?
Since it is a reference, it probably doesn't.

Could it do some processing on this? Yes.

It could say "yay or nay, or maybe". 
- Yes:
	- It looks like a good request. That account holder looks like a real, authorized person on the account, and the account they are referencing is, as of the version I have, good for this kind of thing.
	- I validated the information they provided for the new signator.
	- The end result is a "`RequestToAddSignatoryToAccountCreated`".
	- Who can process this and make it "real"?
- Nope:
	- Something is off, fishy, or whatever.
	- I may log this, too - `RequestToAddSignatoryDenied`"
- Maybe:
	- I have a business rule that says I  have to have account information (AccountId) that is less than (X seconds/minutes/whatever) old. Mine is stale. I'll have to revalidate and get back to you.
	- I have a rule that says each account can only have 10 signators, and this would be 11. But you are a customer with a huge balance, so I'm going to ask for an exception (avoiding "I'd like to but the computer won't let me" hell.)
