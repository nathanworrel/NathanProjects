# 📘 Fakebook Data Structures – User & Social Analytics

This project defines a comprehensive set of data structures for modeling users, photos, relationships, and analytics within a social network called **Fakebook**. It serves as the foundational layer for answering various queries about user demographics, shared photos, friend networks, and event distributions.

---

## 🧱 Project Overview

The project is implemented in **Java** and centers around modeling Fakebook data to support a series of complex social analytics queries. The following types of information are represented:

- User identities
- Tagged photos
- Mutual friend connections
- Potential sibling relationships
- Birth month and name distributions
- Event location statistics
- Age-related friend comparisons

Each class is tailored to support specific types of queries that might be issued in a social network context.

---
## 🧠 Key Features

- Designed for **query modularity** — each class supports a specific type of analysis
- Rich **string formatting** for clean and human-readable outputs
- Uses a custom `FakebookArrayList` to format user and photo lists with separators
- Immutability enforced through `final` classes for data integrity

---

## 💡 Skills Demonstrated

- Object-Oriented Programming (OOP) in Java
- Custom data modeling for large-scale data analysis
- Clean encapsulation and class design
- Advanced use of formatted output for reporting
- Domain modeling for social networks

---

## 📌 Example Queries Enabled by These Structures

- Who shares the most photos together but aren’t friends?
- Which states host the most events?
- Who has the longest or most common first name?
- Which users might be siblings?
- Who is someone’s oldest or youngest friend?

---

## 🚀 How To Use

These classes are intended to be used within a broader Fakebook analytics engine where:
- Data is loaded and parsed
- Queries are issued using the above models
- Results are presented in a readable format via `toString()` methods

---

## 📁 Package
```java
package project2;

