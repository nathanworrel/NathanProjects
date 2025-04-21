# Summary: Hash Join Implementation Project

## Overview

This project involved implementing core components of a hash join algorithm, focusing on efficient disk and memory usage. The goal was to simulate and understand the behavior of database join operations, especially for large datasets that exceed main memory capacity.

## Key Concepts Learned

### 1. **Hash Join Algorithm**
- **Partition Phase**: 
  - Relations are hashed into `buckets` using a hash function.
  - Each bucket corresponds to a partition containing records that share the same hash.
- **Probe Phase**:
  - For each partition, matching records from the left and right relations are joined in-memory using a second hash function.

### 2. **Memory Management**
- Learned how to interact with limited memory (`Mem` class).
- Practiced flushing and loading pages between memory and disk.
- Understood the cost of disk I/O and the importance of minimizing it.

### 3. **Disk Simulation**
- Simulated persistent storage using the `Disk` class.
- Implemented reading from and writing to virtual disk pages.
- Gained insight into how data is stored and retrieved in a paginated structure.

### 4. **Data Structures**
- **Page**:
  - Holds a set number of `Record` objects.
  - Enforces capacity limits and supports inserting and resetting.
- **Record**:
  - Consists of a key and data payload.
  - Supports hashing and comparison operations.
- **Bucket**:
  - Collects page IDs for left and right relation partitions.
  - Tracks the number of records in each partition.

### 5. **Code Architecture**
- Separation of concerns through modular headers (`Bucket.hpp`, `Disk.hpp`, `Join.hpp`, etc.).
- Use of classes to encapsulate disk, memory, page, and record behavior.
- Learned to manage inter-class interactions (e.g., `Bucket` relying on `Disk`).

### 6. **Practical C++ Skills**
- Usage of `std::vector`, `std::shared_ptr`, and other STL constructs.
- Header guards and proper class encapsulation.
- Constructor initialization, class member access, and method definition patterns.

## Files and Their Purpose

| File | Description |
|------|-------------|
| `Record.hpp` | Defines the `Record` structure with hashing and comparison logic. |
| `Page.hpp` | Simulates a memory/disk page to store multiple records. |
| `Disk.hpp` | Manages storage and retrieval of `Page` objects on virtual disk. |
| `Mem.hpp` | Simulates RAM with fixed-size pages, handles flushing/loading. |
| `Bucket.hpp` | Represents a bucket for partitioned join, stores page IDs. |
| `Join.hpp` | Interface for `partition` and `probe` functions to be implemented. |

## Next Steps / Improvements

- Implement actual `partition()` and `probe()` logic in `Join.cpp`.
- Test with larger datasets to simulate realistic database join behavior.
- Optimize hash functions and memory usage patterns.
- Explore handling of skewed data and bucket overflow.

---

This project served as a foundational exercise in understanding how database systems process join operations efficiently, especially under memory constraints. It reinforced core systems programming principles and algorithmic thinking.

