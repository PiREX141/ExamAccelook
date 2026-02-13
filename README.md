# Accelook
Back End Exam for Full-Stack Developer Intern training at Accelist Lentera Indonesia.

Name  : Vincent Anthony Gunawan
NIM   : 2702212143

## Database Creation
### ERD Diagram
![ERD Acceloka](https://github.com/user-attachments/assets/0eab46f7-462a-40ad-875c-fa4d79ad2576)

### Tables in the Database:
1. Category
   <img width="563" height="170" alt="image" src="https://github.com/user-attachments/assets/8600b7cd-edc8-4a1e-b510-17496b1a3ff6" />

2. Ticket
   <img width="714" height="370" alt="image" src="https://github.com/user-attachments/assets/24d16e19-5c60-4f4b-8e7c-e89f2a4efdc6" />

3. BookedTicket
   <img width="606" height="195" alt="image" src="https://github.com/user-attachments/assets/d36a2f54-0ba4-41e6-a03f-5fc42e297c47" />

4. BookingDetail
   <img width="845" height="343" alt="image" src="https://github.com/user-attachments/assets/d35d0c0c-44ec-4bee-b30d-a789e7f82bf9" />

### Notes:
- The database used in the project is SQLServer.
- The _LastSequenceNumber_ in **Category** Table and _SequenceNumber_ in **Ticket** table is used for _TicketCode_ creation.
  - _SequenceNumber_ is used to mark the sequence number of a _Category's_ prefix in **Ticket** table. Example:
     - "C001" would have the _SequenceNumber_ of 1
     - "TD002" would have the _SequenceNumber_ of 2
  - _LastSequenceNumber_ is used to generate the next _SequenceNumber_ when creating a new _TicketCode_. It is updated when a new _TicketCode_ for a category has been created. Example:
     - Currently, the database has 0 data.
     - A new **Ticket** is being created for a _CategoryName_ "Cinema" for the first time. The value of _LastSequenceNumber_ was 0.
     - After the new **Ticket** has been created with _TicketCode_ "C001", the _LastSequenceNumber_ will be updated to 1.
     - Another **Ticket** for "Cinema is being created. The _TicketCode_ is "C002", the _LastSequenceNumber_ will be updated to 2.
  - Both are used to ensure _TicketCode_ increments are based on the _CategoryName_. Example:
     - A database already has "C001" for "Cinema" Ticket.
     - If a new data is being inserted for "Transportasi Darat", the result will be "TD001" and not "TD002".
- The _LastSequenceNumber_ and _SequenceNumber_ don't really need to be implemented fully in this case since there are no POST Ticket requirements.
- There was an error in the database where **BookingDetail** Table references **Ticket** Table. The reference should be placed in the **BookedTicket** Table, but due to time constraints, it wasn't modified.
- For the complete Database Creation queries, they can be found in _Initial.sql_ file inside _ExamAccelook.Entities_.

 ## MARVEL Pattern
 MARVEL Pattern usage reference: https://github.com/accelist/Accelist.WebApiStandard

 ### Layers in the program
 - ExamAccelook = Contains program.cs and Controllers for API Endpoints.
 - ExamAccelook.Entities = Contains Entities from the database via scaffolding.
 - ExamAccelook.Contracts = Contains Requests and Responses for each API Endpoint. 
 - ExamAccelook.Logics = Contains Handlers and Validators logic.







































