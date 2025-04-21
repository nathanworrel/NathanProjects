/**
 * Project 2
 * LC-2K Linker
 */

#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#define MAXSIZE 300
#define MAXLINELENGTH 1000
#define MAXFILES 6

typedef struct FileData FileData;
typedef struct SymbolTableEntry SymbolTableEntry;
typedef struct RelocationTableEntry RelocationTableEntry;
typedef struct CombinedFiles CombinedFiles;

struct SymbolTableEntry
{
	char label[7];
	char location;
	int offset;
};

struct RelocationTableEntry
{
	int offset;
	char inst[7];
	char label[7];
	int file;
};

struct FileData
{
	int textSize;
	int dataSize;
	int symbolTableSize;
	int relocationTableSize;
	int textStartingLine; // in final executable
	int dataStartingLine; // in final executable
	int text[MAXSIZE];
	int data[MAXSIZE];
	SymbolTableEntry symbolTable[MAXSIZE];
	RelocationTableEntry relocTable[MAXSIZE];
	int distanceFD;
	int distanceFS;
};

struct CombinedFiles
{
	int text[MAXSIZE];
	int data[MAXSIZE];
	SymbolTableEntry symTable[MAXSIZE];
	RelocationTableEntry relocTable[MAXSIZE];
	int textSize;
	int dataSize;
	int symTableSize;
	int relocTableSize;
};

struct Symbol
{
	char label[7];
	int offset;
};

int findLabels(struct Symbol GLabels[MAXSIZE], char *l, int s);

int main(int argc, char *argv[])
{
	char *inFileString, *outFileString;
	FILE *inFilePtr, *outFilePtr;
	int i, j;
	int numF = argc - 2;
	int stack = 0;
	static struct Symbol GLabels[MAXSIZE];

	if (argc <= 2)
	{
		printf("error: usage: %s <obj file> ... <output-exe-file>\n",
			   argv[0]);
		exit(1);
	}

	outFileString = argv[argc - 1];

	outFilePtr = fopen(outFileString, "w");
	if (outFilePtr == NULL)
	{
		printf("error in opening %s\n", outFileString);
		exit(1);
	}

	FileData files[MAXFILES];

	// read in all files and combine into a "master" file
	for (i = 0; i < numF; i++)
	{
		inFileString = argv[i + 1];

		inFilePtr = fopen(inFileString, "r");
		printf("opening %s\n", inFileString);

		if (inFilePtr == NULL)
		{
			printf("error in opening %s\n", inFileString);
			exit(1);
		}

		char line[MAXLINELENGTH];
		int sizeText, sizeData, sizeSymbol, sizeReloc;

		// parse first line of file
		fgets(line, MAXSIZE, inFilePtr);
		sscanf(line, "%d %d %d %d",
			   &sizeText, &sizeData, &sizeSymbol, &sizeReloc);

		stack += sizeText;
		stack += sizeData;

		files[i].textSize = sizeText;
		files[i].dataSize = sizeData;
		files[i].symbolTableSize = sizeSymbol;
		files[i].relocationTableSize = sizeReloc;
		files[i].distanceFD = sizeText;
		files[i].distanceFS = 0;

		// read in text section
		int instr;
		for (j = 0; j < sizeText; j++)
		{
			fgets(line, MAXLINELENGTH, inFilePtr);
			instr = atoi(line);
			files[i].text[j] = instr;
		}

		// read in data section
		int data;
		for (j = 0; j < sizeData; j++)
		{
			fgets(line, MAXLINELENGTH, inFilePtr);
			data = atoi(line);
			files[i].data[j] = data;
		}

		// read in the symbol table
		char label[7];
		char type;
		int addr;
		for (j = 0; j < sizeSymbol; j++)
		{
			fgets(line, MAXLINELENGTH, inFilePtr);
			sscanf(line, "%s %c %d",
				   label, &type, &addr);
			files[i].symbolTable[j].offset = addr;
			strcpy(files[i].symbolTable[j].label, label);
			files[i].symbolTable[j].location = type;
		}

		// read in relocation table
		char opcode[7];
		for (j = 0; j < sizeReloc; j++)
		{
			fgets(line, MAXLINELENGTH, inFilePtr);
			sscanf(line, "%d %s %s",
				   &addr, opcode, label);
			files[i].relocTable[j].offset = addr;
			strcpy(files[i].relocTable[j].inst, opcode);
			strcpy(files[i].relocTable[j].label, label);
			files[i].relocTable[j].file = i;
		}
		fclose(inFilePtr);

		for (int k = 0; k < i; ++k)
		{
			files[k].distanceFD += files[i].textSize;
			files[i].distanceFS += files[k].textSize;
			files[i].distanceFD += files[k].dataSize;
		}

	} // end reading files

	// *** INSERT YOUR CODE BELOW ***
	//    Begin the linking process
	//    Happy coding!!!
	// sad nathan starts here
	int count = 0;
	for (i = 0; i < numF; ++i)
	{
		for (j = 0; j < files[i].symbolTableSize; ++j)
		{
			if (!strcmp(files[i].symbolTable[j].label, "Stack") && files[i].symbolTable[j].location != 'U')
			{
				printf("error: defined Stack\n");
				exit(1);
			}
			int hold = 0;
			if(files[i].symbolTable[j].location == 'D')
			{
				hold = files[i].distanceFS + files[i].distanceFD + files[i].symbolTable[j].offset;
			}
			else if(files[i].symbolTable[j].location == 'T')
			{
				hold = files[i].distanceFS + files[i].symbolTable[j].offset;
			}
			else
			{
				continue;
			}
			strcpy(GLabels[count].label, files[i].symbolTable[j].label);
			GLabels[count].offset = hold;
			if(findLabels(GLabels, GLabels[count].label, stack) != hold)
			{
				printf("error: double defined global variable\n");
				exit(1);
			}
			count++;
		}
	}

	for (i = 0; i < numF; ++i)
	{
		for (j = 0; j < files[i].relocationTableSize; ++j)
		{
			if (files[i].relocTable[j].label[0] >= 'A' && files[i].relocTable[j].label[0] <= 'Z')
			{
				if (files[i].relocTable[j].inst[0] == '.')
				{
					files[i].data[files[i].relocTable[j].offset] = files[i].data[files[i].relocTable[j].offset] >> 16;
					files[i].data[files[i].relocTable[j].offset] = files[i].data[files[i].relocTable[j].offset] << 16;
					files[i].data[files[i].relocTable[j].offset] += findLabels(GLabels, files[i].relocTable[j].label, stack);
				}
				else
				{
					files[i].text[files[i].relocTable[j].offset] = files[i].text[files[i].relocTable[j].offset] >> 16;
					files[i].text[files[i].relocTable[j].offset] = files[i].text[files[i].relocTable[j].offset] << 16;
					files[i].text[files[i].relocTable[j].offset] += findLabels(GLabels, files[i].relocTable[j].label, stack);
				}
				if(files[i].relocTable[j].inst[0] == 'b')
				{
					files[i].text[files[i].relocTable[j].offset] -= (files[i].relocTable[j].offset + files[i].distanceFS);
				}
			}
			else
			{
				if (files[i].relocTable[j].inst[0] == '.')
				{
					files[i].data[files[i].relocTable[j].offset] += files[i].distanceFS;
				}
				else if(files[i].relocTable[j].inst[0] == 'b')
				{
					continue;
				}
				else
				{
					files[i].text[files[i].relocTable[j].offset] += (files[i].distanceFS + files[i].distanceFD - files[i].textSize);
				}
			}
			
		}
	}

	//Prints the output correctly :)
	for (i = 0; i < numF; ++i)
	{
		for (j = 0; j < files[i].textSize; ++j)
		{
			fprintf(outFilePtr, "%d", files[i].text[j]);
			fprintf(outFilePtr, "\n");
		}
	}
	for (i = 0; i < numF; ++i)
	{
		for (j = 0; j < files[i].dataSize; ++j)
		{
			fprintf(outFilePtr, "%d", files[i].data[j]);
			fprintf(outFilePtr, "\n");
		}
	}
	return 0;

} // main

int findLabels(struct Symbol GLabels[MAXSIZE], char *l, int s)
{
	for (int i = 0; i < MAXSIZE; ++i)
	{
		if (!strcmp(GLabels[i].label, l))
		{
			return GLabels[i].offset;
		}
	}
	if(!strcmp(l, "Stack"))
	{
		return s;
	}
	printf("error: no global label\n");
	exit(1);
}