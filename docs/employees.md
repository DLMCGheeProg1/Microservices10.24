

# Employee Hired

Operation: EmployeeHired
Operands: 
    EmployeeId: string
    Name: string


Operation: EmployeeAssignedToDepartment
    Operands:
        ref: EmployeeId
        Department: string

Operation: EmployeeSalaryAdjustment
    Operands:
        ref: EmployeeId
        Amount: 120000

