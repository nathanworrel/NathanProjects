/**
 * Project 1 
 * Assembler code fragment for LC-2K 
 */

#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#define MAXLINELENGTH 1000
#define maxLines 65536
#define nul '/0'
char zer = '0';

int readAndParse(FILE *, char *, char *, char *, char *, char *);
int isNumber(char *);

int checkOff(char *f, char *s, char *t, char labels[maxLines][8]);

int addParts(char *f, char *s, char *t, char labels[maxLines][8]);

int findLabels(char labels[maxLines][8], char *l);

struct entry
{
    char label[MAXLINELENGTH];
    char LO[MAXLINELENGTH];
    int off;
};

int newS(char a[MAXLINELENGTH], int b, struct entry c[maxLines]);

int main(int argc, char *argv[])
{
    char *inFileString, *outFileString;
    FILE *inFilePtr, *outFilePtr;
    char label[MAXLINELENGTH], opcode[MAXLINELENGTH], arg0[MAXLINELENGTH],
        arg1[MAXLINELENGTH], arg2[MAXLINELENGTH];

    if (argc != 3)
    {
        printf("error: usage: %s <assembly-code-file> <machine-code-file>\n",
               argv[0]);
        exit(1);
    }

    inFileString = argv[1];
    outFileString = argv[2];

    inFilePtr = fopen(inFileString, "r");
    if (inFilePtr == NULL)
    {
        printf("error in opening %s\n", inFileString);
        exit(1);
    }
    outFilePtr = fopen(outFileString, "w");
    if (outFilePtr == NULL)
    {
        printf("error in opening %s\n", outFileString);
        exit(1);
    }
    char labels[maxLines][8];
    int nums[maxLines];
    static struct entry symbol[maxLines];
    static struct entry reloc[maxLines];
    int num = 0;

    int numT = 0;
    int numD = 0;
    int numS = 0;
    int numR = 0;

    while (readAndParse(inFilePtr, label, opcode, arg0, arg1, arg2))
    {
        if (label[0] != '\0')
        {

            for (int i = 0; i < num; ++i)
            {
                if (!strcmp(labels[i], label))
                {
                    printf("error repeat label\n");
                    exit(1);
                }
            }
            if (label[0] >= 'A' && label[0] <= 'Z')
            {
                if (strcmp(opcode, ".fill"))
                {
                    strcpy(symbol[numS].label, label);
                    strcpy(symbol[numS].LO, "T");
                    symbol[numS].off = numT;
                }
                else
                {
                    strcpy(symbol[numS].label, label);
                    strcpy(symbol[numS].LO, "D");
                    symbol[numS].off = numD;
                }
                ++numS;
            }

            strcpy(labels[num], label);
        }
        if (!strcmp(opcode, ".fill"))
        {
            ++numD;
        }
        else
        {
            ++numT;
        }

        ++num;
    }

    rewind(inFilePtr);

    num = -1;

    while (readAndParse(inFilePtr, label, opcode, arg0, arg1, arg2))
    {
        num++;
        int hold = 0;
        if (!strcmp(opcode, "add"))
        {
            // good
            hold += (0 << 22);
            hold += addParts(arg0, arg1, arg2, labels);
        }
        else if (!strcmp(opcode, "nor"))
        {
            // good
            hold += (1 << 22);
            hold += addParts(arg0, arg1, arg2, labels);
        }
        else if (!strcmp(opcode, "lw"))
        {
            // add code
            hold += (2 << 22);
            hold += checkOff(arg0, arg1, arg2, labels);
            if (!isNumber(arg2))
            {
                reloc[numR].off = num;
                strcpy(reloc[numR].LO, opcode);
                strcpy(reloc[numR].label, arg2);
                ++numR;

                if (arg2[0] >= 'A' && arg2[0] <= 'Z')
                {
                    if (newS(arg2, numS, symbol))
                    {
                        strcpy(symbol[numS].label, arg2);
                        strcpy(symbol[numS].LO, "U");
                        symbol[numS].off = 0;
                        ++numS;
                    }
                }
            }
        }
        else if (!strcmp(opcode, "sw"))
        {
            // add code
            hold += (3 << 22);
            hold += checkOff(arg0, arg1, arg2, labels);
            if (!isNumber(arg2))
            {
                reloc[numR].off = num;
                strcpy(reloc[numR].LO, opcode);
                strcpy(reloc[numR].label, arg2);
                ++numR;

                if (arg2[0] >= 'A' && arg2[0] <= 'Z')
                {
                    if (newS(arg2, numS, symbol))
                    {
                        strcpy(symbol[numS].label, arg2);
                        strcpy(symbol[numS].LO, "U");
                        symbol[numS].off = 0;
                        ++numS;
                    }
                }
            }
        }
        else if (!strcmp(opcode, "beq"))
        {
            // add code
            char h[MAXLINELENGTH];
            if (isNumber(arg2))
            {
                sprintf(h, "%d", (atoi(arg2)));
            }
            else
            {
                if (arg2[0] >= 'A' && arg2[0] <= 'Z')
                {
                    if (newS(arg2, numS, symbol))
                    {
                        printf("error: no valid symbol\n");
                        exit(1);
                    }
                }
                sprintf(h, "%d", ((findLabels(labels, arg2)) - (1 + num)));
            }
            hold += (4 << 22);
            hold += checkOff(arg0, arg1, h, labels);
        }
        else if (!strcmp(opcode, "jalr"))
        {
            // add code
            hold += (5 << 22);
            hold += addParts(arg0, arg1, &zer, labels);
        }
        else if (!strcmp(opcode, "halt"))
        {
            // good
            hold += (6 << 22);
        }
        else if (!strcmp(opcode, "noop"))
        {
            // good
            hold += (7 << 22);
        }
        else if (!strcmp(opcode, ".fill"))
        {
            // good
            hold += addParts(&zer, &zer, arg0, labels);
            if (!isNumber(arg0))
            {
                reloc[numR].off = num - numT;
                strcpy(reloc[numR].LO, opcode);
                strcpy(reloc[numR].label, arg0);
                ++numR;
                if (arg0[0] >= 'A' && arg0[0] <= 'Z')
                {
                    if (newS(arg0, numS, symbol))
                    {
                        strcpy(symbol[numS].label, arg0);
                        strcpy(symbol[numS].LO, "U");
                        symbol[numS].off = 0;
                        ++numS;
                    }
                }
            }
        }
        else
        {
            printf("error no valid opcode\n");
            exit(1);
        }

        nums[num] = hold;
    }

    fprintf(outFilePtr, "%d %d %d %d", numT, numD, numS, numR);
    fprintf(outFilePtr, "\n");
    for (int i = 0; i < num + 1; ++i)
    {
        fprintf(outFilePtr, "%d", nums[i]);
        fprintf(outFilePtr, "\n");
    }
    for (int i = 0; i < numS; ++i)
    {
        fprintf(outFilePtr, "%s %s %d", symbol[i].label, symbol[i].LO, symbol[i].off);
        fprintf(outFilePtr, "\n");
    }
    for (int i = 0; i < numR; ++i)
    {
        fprintf(outFilePtr, "%d %s %s", reloc[i].off, reloc[i].LO, reloc[i].label);
        fprintf(outFilePtr, "\n");
    }

    return (0);
}

/*
 * Read and parse a line of the assembly-language file.  Fields are returned
 * in label, opcode, arg0, arg1, arg2 (these strings must have memory already
 * allocated to them).
 *
 * Return values:
 *     0 if reached end of file
 *     1 if successfully read
 *
 * exit(1) if line is too long.
 */
int readAndParse(FILE *inFilePtr, char *label, char *opcode, char *arg0,
                 char *arg1, char *arg2)
{
    char line[MAXLINELENGTH];

    /* delete prior values */
    label[0] = opcode[0] = arg0[0] = arg1[0] = arg2[0] = '\0';

    /* read the line from the assembly-language file */
    if (fgets(line, MAXLINELENGTH, inFilePtr) == NULL)
    {
        /* reached end of file */
        return (0);
    }

    /* check for line too long (by looking for a \n) */
    if (strchr(line, '\n') == NULL)
    {
        /* line too long */
        printf("error: line too long\n");
        exit(1);
    }

    /* is there a label? */
    char *ptr = line;
    if (sscanf(ptr, "%[^\t\n\r ]", label))
    {
        /* successfully read label; advance pointer over the label */
        ptr += strlen(label);
    }

    /*
     * Parse the rest of the line.  Would be nice to have real regular
     * expressions, but scanf will suffice.
     */
    sscanf(ptr, "%*[\t\n\r ]%[^\t\n\r ]%*[\t\n\r ]%[^\t\n\r ]%*[\t\n\r ]%[^\t\n\r ]%*[\t\n\r ]%[^\t\n\r ]",
           opcode, arg0, arg1, arg2);
    return (1);
}

int isNumber(char *string)
{
    /* return 1 if string is a number */
    int i;
    return ((sscanf(string, "%d", &i)) == 1);
}

int checkOff(char *f, char *s, char *t, char labels[maxLines][8])
{
    int hold = 0;

    if (isNumber(t))
    {
        hold += (atoi(t));
        if (hold < -32768 || hold > 32767)
        {
            printf("error: out of scope\n");
            exit(1);
        }
    }
    else
    {
        hold += (findLabels(labels, t));
    }
    if (hold < 0)
    {
        hold = hold & ((1 << 16) - 1);
    }
    return addParts(f, s, &zer, labels) + hold;
}

int addParts(char *f, char *s, char *t, char labels[maxLines][8])
{
    int hold = 0;
    if (isNumber(f))
    {
        hold += (atoi(f) << 19);
    }
    else
    {
        hold += (findLabels(labels, f) << 19);
    }
    if (isNumber(s))
    {
        hold += (atoi(s) << 16);
    }
    else
    {
        hold += (findLabels(labels, s) << 16);
    }
    if (isNumber(t))
    {
        hold += (atoi(t));
    }
    else
    {
        hold += (findLabels(labels, t));
    }
    return hold;
}

int findLabels(char labels[maxLines][8], char *l)
{
    for (int i = 0; i < maxLines; ++i)
    {
        if (!strcmp(labels[i], l))
        {
            return i;
        }
    }
    if (l[0] >= 'A' && l[0] <= 'Z')
    {
        return 0;
    }
    printf("error: no local label\n");
    exit(1);
}

int newS(char a[MAXLINELENGTH], int b, struct entry c[maxLines])
{
    for (int i = 0; i < b; ++i)
    {
        if (!strcmp(c[i].label, a))
        {
            return 0;
        }
    }
    return 1;
}