# PromotionEngine
PromotionEngine is implemented using TDD approach. This is a .Net Standard library which is built as a promotion engine to cater the following problem statement,

**Problem Statement:**

- Implement a simple promotion engine for a checkout process. Cart contains a list of single character SKU ids (A,B,C,..) over which the promotion engine will run.

- The promotion engine need to calculate total order value after applying the 2 promotion types,
  - buy 'n' items of a SKU for a fixed price (3 A's for 130)
  - buySKU 1 & SKU 2 for a fixed price (C + D = 30)

- The promotion engine needs to be modular to allow for more promotion types to be added at a later date (e.g. a future promotion could be x% of a SKU unit price). 
Promotions will be mutually exclusive i.e. if one is applied the other promotions can't be applied.

**Test Setup:**

Unit price for SKU IDs

SKU  | Unit Price
---- | -----------
A    | 50
B    | 30
C    | 20
D    | 15

Active Promotions
- 3 of A's for 130
- 2 of B's for 45
- C & D for 30

**Scenario A**

SKU   | Unit Price
----- | -----------
1 * A | 50
1 * B | 30
1 * C | 20
Total | 100

**Scenario B**

SKU   | Unit Price
----- | -----------
5 * A | 130 + 2*50
5 * B | 45 + 45 + 30
1 * C | 20
Total | 370

**Scenario C**

SKU   | Unit Price
----- | -----------
3 * A | 130
5 * B | 45 + 45 + 1*30 
1 * C | -
1 * D | 30
Total | 280
