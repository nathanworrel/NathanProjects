/**
 * Project 1
 */

#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#define NUMMEMORY 65536 /* maximum number of words in memory */
#define NUMREGS 8       /* number of machine registers */
#define MAXLINELENGTH 1000

typedef struct stateStruct
{
    int pc;
    int mem[NUMMEMORY];
    int reg[NUMREGS];
    int numMemory;
} stateType;

typedef struct instStruct
{
    int opcode;
    int rega;
    int regb;
    int regc;
} inst;

void printState(stateType *);
void getAddress(inst *pass, int mem);
int convertNum(int);

int main(int argc, char *argv[])
{
    char line[MAXLINELENGTH];
    stateType state;
    FILE *filePtr;

    if (argc != 2)
    {
        printf("error: usage: %s <machine-code file>\n", argv[0]);
        exit(1);
    }

    filePtr = fopen(argv[1], "r");
    if (filePtr == NULL)
    {
        printf("error: can't open file %s", argv[1]);
        perror("fopen");
        exit(1);
    }

    /* read the entire machine-code file into memory */
    for (state.numMemory = 0; fgets(line, MAXLINELENGTH, filePtr) != NULL;
         state.numMemory++)
    {

        if (sscanf(line, "%d", state.mem + state.numMemory) != 1)
        {
            printf("error in reading address %d\n", state.numMemory);
            exit(1);
        }
        printf("memory[%d]=%d\n", state.numMemory, state.mem[state.numMemory]);
    }
    for(int i = 0; i < NUMREGS; ++i)
    {
        state.reg[i] = 0;
    }

    state.pc = 0;
    int num = 0;

    while(0 == 0)
    {
        printState(&state);
        inst i;
        getAddress(&i, state.mem[state.pc]);
        num ++;
        if(i.opcode == 0)
        {
            //add
            state.reg[i.regc] = state.reg[i.rega] + state.reg[i.regb];
            state.pc++;
        }
        else if(i.opcode == 1)
        {
            //nor
            state.reg[i.regc] = ~(state.reg[i.rega] | state.reg[i.regb]);
            state.pc++;
        }
        else if(i.opcode == 2)
        {
            //lw
            state.reg[i.regb] = state.mem[state.reg[i.rega] + convertNum(i.regc)];
            state.pc ++;
        }
        else if(i.opcode == 3)
        {
            //sw
            state.pc ++;
            state.mem[state.reg[i.rega] + convertNum(i.regc)] = state.reg[i.regb];
        }
        else if(i.opcode == 4)
        {
            //beq
            if(state.reg[i.rega] == state.reg[i.regb])
            {
                state.pc += 1 + convertNum(i.regc);
            }
            else
            {
                state.pc ++;
            }
        }
        else if(i.opcode == 5)
        {
            //jalr
            state.reg[i.regb] = state.pc + 1;
            state.pc = state.reg[i.rega];
        }
        else if(i.opcode == 6)
        {
            //hault or the end of the program
            printf("machine halted\n");
            printf("total of %i instructions executed\n", num);
            printf("final state of machine:\n");
            state.pc ++;
            printState(&state);
            break;
        }
        else if(i.opcode == 7)
        {
            // Nothing happens
            state.pc ++;
        }
        else
        {
            printf("error no valid opcode\n");
            exit(1);
        }

    }

    return (0);
}

void printState(stateType *statePtr)
{
    int i;
    printf("\n@@@\nstate:\n");
    printf("\tpc %d\n", statePtr->pc);
    printf("\tmemory:\n");
    for (i = 0; i < statePtr->numMemory; i++)
    {
        printf("\t\tmem[ %d ] %d\n", i, statePtr->mem[i]);
    }
    printf("\tregisters:\n");
    for (i = 0; i < NUMREGS; i++)
    {
        printf("\t\treg[ %d ] %d\n", i, statePtr->reg[i]);
    }
    printf("end state\n");
}

int convertNum(int num)
{
    /* convert a 16-bit number into a 32-bit Linux integer */
    if (num & (1 << 15))
    {
        num -= (1 << 16);
    }
    return (num);
}

void getAddress(inst *pass, int mem)
{
    int h = mem;
    pass->opcode = h >> 22;
    h -= pass->opcode << 22;
    pass->rega = h >> 19;
    h -= pass->rega << 19;
    pass->regb = h >> 16;
    h -= pass->regb << 16;
    pass->regc = h;
    /*
    printf("opcode: %i\n", pass->opcode);
    printf("rega: %i\n",pass->rega);
    printf("regb: %i\n",pass->regb);
    printf("regc: %i\n",pass->regc);
    */
}
