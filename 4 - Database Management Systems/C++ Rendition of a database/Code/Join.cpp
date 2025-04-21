#include "Join.hpp"

#include <vector>

using namespace std;

/*
 * Input: Disk, Memory, Disk page ids for left relation, Disk page ids for right relation
 * Output: Vector of Buckets of size (MEM_SIZE_IN_PAGE - 1) after partition
 */
vector<Bucket> partition(Disk* disk, Mem* mem, pair<uint, uint> left_rel,
                         pair<uint, uint> right_rel) {
	vector<Bucket> partitions(MEM_SIZE_IN_PAGE-1, Bucket(disk));
	mem->reset();
	//Left Partition
	for(uint i = left_rel.first; i < left_rel.second; i++){
		mem->loadFromDisk(disk, i, MEM_SIZE_IN_PAGE-1);
		Page* hold = mem->mem_page(MEM_SIZE_IN_PAGE-1);
		for(uint j = 0; j < hold->size(); j++){
			Record temp = hold->get_record(j);
			uint mem_page = temp.partition_hash()%(MEM_SIZE_IN_PAGE-1);
			mem->mem_page(mem_page)->loadRecord(temp);
			if(mem->mem_page(mem_page)->full()){
				partitions[mem_page].add_left_rel_page(mem->flushToDisk(disk, mem_page));
			}
		}
	}
	for(uint i = 0; i < MEM_SIZE_IN_PAGE-1; i++){
		if(mem->mem_page(i)->size() !=0){
			partitions[i].add_left_rel_page(mem->flushToDisk(disk, i));
		}
	}

	mem->reset();
	//right partition
	for(uint i = right_rel.first; i < right_rel.second; i++){
		mem->loadFromDisk(disk, i, MEM_SIZE_IN_PAGE-1);
		Page* hold = mem->mem_page(MEM_SIZE_IN_PAGE-1);
		for(uint j = 0; j < hold->size(); j++){
			Record temp = hold->get_record(j);
			uint mem_page = temp.partition_hash()%(MEM_SIZE_IN_PAGE-1);
			mem->mem_page(mem_page)->loadRecord(temp);
			if(mem->mem_page(mem_page)->full()){
				partitions[mem_page].add_right_rel_page(mem->flushToDisk(disk, mem_page));
			}
		}
	}
	for(uint i = 0; i < MEM_SIZE_IN_PAGE-1; i++){
		if(mem->mem_page(i)->size() !=0){
			partitions[i].add_right_rel_page(mem->flushToDisk(disk, i));
		}
	}
	
	return partitions;
}

/*
 * Input: Disk, Memory, Vector of Buckets after partition
 * Output: Vector of disk page ids for join result
 */
vector<uint> probe(Disk* disk, Mem* mem, vector<Bucket>& partitions) {
	// TODO: implement probe phase
	vector<uint> disk_pages;
	mem->reset();
	for(Bucket bucket: partitions){
		for(uint i = 0; i < MEM_SIZE_IN_PAGE -2; i++){
			mem->mem_page(i)->reset();
		}
		vector<uint> small = bucket.get_left_rel();
		vector<uint> right = bucket.get_right_rel();
		if(small.size() > right.size()){
			small.swap(right);
		}
		for(uint i = 0; i < small.size(); i++){
			mem->loadFromDisk(disk, small[i], MEM_SIZE_IN_PAGE-2);
			Page* hold = mem->mem_page(MEM_SIZE_IN_PAGE-2);
			for(uint j = 0; j < hold->size(); j++){
				Record temp = hold->get_record(j);
				mem->mem_page(temp.probe_hash()%(MEM_SIZE_IN_PAGE-2))->loadRecord(temp);
			}
		}
		for(uint i = 0; i < right.size(); i++){
			mem->loadFromDisk(disk, right[i], MEM_SIZE_IN_PAGE-2);
			Page* hold = mem->mem_page(MEM_SIZE_IN_PAGE-2);
			for(uint j = 0; j < hold->size(); j++){
				Record temp = hold->get_record(j);
				uint hash = temp.probe_hash()%(MEM_SIZE_IN_PAGE-2);
				Page* page = mem->mem_page(hash);
				for(uint k = 0; k < page->size(); k++){
					Record comp = page->get_record(k);
					if(comp == temp){
						mem->mem_page(MEM_SIZE_IN_PAGE-1)->loadPair(comp,temp);
						if(mem->mem_page(MEM_SIZE_IN_PAGE-1)->full()){
							disk_pages.push_back(mem->flushToDisk(disk, MEM_SIZE_IN_PAGE-1));
						}
					}
				}
			}
		}
	}
	if(mem->mem_page(MEM_SIZE_IN_PAGE-1)->size() != 0){
		disk_pages.push_back(mem->flushToDisk(disk, MEM_SIZE_IN_PAGE-1));
	}
	return disk_pages;
}
