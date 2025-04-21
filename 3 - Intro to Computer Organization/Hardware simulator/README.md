# 🖥️ LC-2K Hardware Simulator in C

This project is a **hardware simulator** that emulates the instruction-level behavior of a simple computer architecture, based on a subset of the **LC-2K ISA (Instruction Set Architecture)**. Written in C, the simulator loads and executes machine-code files representing compiled assembly instructions.

---

## 🧠 Features

- **Memory Emulation**: Simulates up to `65536` memory words
- **Register File**: Implements 8 general-purpose 32-bit registers
- **Instruction Decoding**: Supports decoding 32-bit LC-2K binary instructions
- **Instruction Execution**: Implements core instructions:
  - `ADD` (opcode 0)
  - `NOR` (opcode 1)
  - `LW`  (Load Word - opcode 2)
  - `SW`  (Store Word - opcode 3)
  - `BEQ` (Branch if Equal - opcode 4)
  - `JALR` (Jump and Link Register - opcode 5)
  - `HALT` (opcode 6)
  - `NOOP` (opcode 7)

- **State Reporting**: Logs the full machine state (PC, memory, and register contents) at every cycle
- **Cycle Tracking**: Tracks and prints the total number of executed instructions

---

## 🧾 Code Structure

- `main()`: Reads machine code from a file, initializes state, and enters the execution loop
- `printState()`: Prints the current program counter, memory, and register states
- `convertNum()`: Converts 16-bit signed offset fields to 32-bit signed values
- `getAddress()`: Parses a 32-bit instruction into its opcode and operands

---

## 🗃️ Memory Layout

- **Instruction Memory**: Machine code instructions loaded into `state.mem[]`
- **Registers**: 8 registers in `state.reg[]`
- **Program Counter (PC)**: Tracked in `state.pc`

---

## 🔧 How It Works

1. Load machine code from file into memory
2. Reset registers and PC
3. For each instruction:
   - Decode the opcode and operands
   - Execute the operation (e.g., arithmetic, memory access, branch)
   - Log the updated machine state
   - Repeat until `HALT`

---

## 🧪 Example Output
```bash
memory[0]=...    ← Loaded instruction
@@@              
state:           
    pc 0         
    memory:      
        mem[0] ... 
        ...
    registers:   
        reg[0] 0  
        ...
end state       
...
machine halted
total of X instructions executed
final state of machine:
...

