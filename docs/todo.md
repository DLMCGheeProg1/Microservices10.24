# Tier 1 Support
- Annotating that they've contacted the customer

```
POST /tierone/submitted-incidents/{id}/contact-records

{
    "note": "Gave them a call"
}

```

```
GET /tierone/submitted-incidents/

```
- Optionally annotating that it is high priority.
- Optionally resolving it.
- Assigning it to a tech
    - What's their process for doing this?

# Back to the Incident
- Deciding if they can delete it:
    - How can we help the employee decide if they can delete it or not?
    - Giving them a list?
    - Only if it is still in the PendingTier1Review State
        - How would they know? (Another read model?)