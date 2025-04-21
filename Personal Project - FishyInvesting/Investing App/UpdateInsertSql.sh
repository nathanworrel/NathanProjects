#!/bin/bash                                                                  

# example usage: bash UpdateInsertSql.sh QQQ 2024-01-01 2024-02-01
                                                                                                                      
symbol=$1                                                                                                             
startDate=$2                                                                                                          
endDate=$3                                                                                                            
                                                                                                                      
startEpoch=$(gdate -d "$startDate" '+%s')                                                                              
endEpoch=$(gdate -d "$endDate" '+%s')

echo $startEpoch
echo $endEpoch                                                                                                                                                                                                               

baseUrl="https://query1.finance.yahoo.com/v7/finance/download/"                                                       
args="$symbol?period1=$startEpoch&period2=$endEpoch&interval=1d&events=history"      
sheetUrl="$baseUrl$args"
response=$(curl $sheetUrl \
  -H 'accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7' \
  -H 'accept-language: en-US,en;q=0.9' \
  -H 'cookie: tbla_id=27d918fe-7cf5-4756-aca1-027c7a9fad91-tuctc64d1e9; axids=gam=y-3SVr12hE2uIWW8QBQjCphDOI1388LsOo~A&dv360=eS1xOVBLSUtaRTJ1RjduVWJuSU9WLlVxNlRVN3ZNTkxHVn5B&ydsp=y-3k0pJ5BE2uLMCyqGWT1iyayCQD_rX8O1~A&tbla=y-iHe.a.lE2uJZoIiAh634a9CWCivDj2L7~A; OTH=v=2&s=2&d=eyJraWQiOiIwMTY0MGY5MDNhMjRlMWMxZjA5N2ViZGEyZDA5YjE5NmM5ZGUzZWQ5IiwiYWxnIjoiUlMyNTYifQ.eyJjdSI6eyJndWlkIjoiQ0ZLTFFBUUNGMlhBTk5NUzYzSDRVUk5RRlUiLCJwZXJzaXN0ZW50Ijp0cnVlLCJzaWQiOiJiN0M0YndDeXNIVlcifX0.OdlYVW9hB3JGoZgsHUkdxSgGBI1mgAnSgZ_dG2mOHSlMdEaLpFqZVjsllmEtDnRq7nnvauzrtSD11YYsmd05a0fUYn6LdmPWhxxHXQvlti3-r1Vm-9OS5RHaxNHH9y5znugzOlNaNMeG6X-mZV9uFlTBvM2SI_A73Eywsx7x5hc; T=af=JnRzPTE3MjAzOTU1NjYmcHM9WG5jai5COFZ2ckR0STNlelJxbWY0Zy0t&d=bnMBeWFob28BZwFDRktMUUFRQ0YyWEFOTk1TNjNINFVSTlFGVQFhYwFBSUJwaTBFWAFhbAFhbm5AdGhlc3RvbmVzLm1lAXNjAW1icl9yZWdpc3RyYXRpb24BZnMBTEQ5MF9EOW1peWN1AXp6AXVjeWltQkE3RQFhAVFBRQFsYXQBdWN5aW1CAW51ATA-&kt=EAAbffnIr7FLLYHDtZBDmie_g--~I&ku=FAAiN7iOAp6273l.nVtxiJqQ.pNg_B.V07ckXidj.pLX6V27ADp9Dk50Inw8jKJ5PoF0fql2mVeDoQEihxjIZdU.iowTHnZCywQN9f8VzyVZQGwSg5pvXzhAMPeH4Z3TS9U6_noI1YGX0KUOsSqcY0QUGYeao3BzZn4.5H_xI77VM0-~E; F=d=UAUXqNQ9vDbYI412E9c.lNx0F7HtUirAF7wUb.6QgWISkcNv; PH=l=en-US; Y=v=1&n=5duvich9n9ue5&l=2j2mdlfwo0f58bdlnf4hgfd7vjsllohian2j0n20/o&p=n35vvvv00000000&r=1dm&intl=us; GUCS=AVOHI3CD; cmp=t=1720395568&j=0&u=1YNN; gpp=DBAA; gpp_sid=-1; GUC=AQEACAJmjHdmtEIbAwQa&s=AQAAAGDHgDVe&g=ZosnPA; A1=d=AQABBGpMa2UCEEqhwVAdv46ntgAx_SIrGB0FEgEACAJ3jGa0ZtxS0iMA_eMBAAcIakxrZSIrGB0ID8nbSEGnDLHS1qXbswA8pwkBBwoBdw&S=AQAAAilXTMo9H0FUKq10tebhtHE; A3=d=AQABBGpMa2UCEEqhwVAdv46ntgAx_SIrGB0FEgEACAJ3jGa0ZtxS0iMA_eMBAAcIakxrZSIrGB0ID8nbSEGnDLHS1qXbswA8pwkBBwoBdw&S=AQAAAilXTMo9H0FUKq10tebhtHE; A1S=d=AQABBGpMa2UCEEqhwVAdv46ntgAx_SIrGB0FEgEACAJ3jGa0ZtxS0iMA_eMBAAcIakxrZSIrGB0ID8nbSEGnDLHS1qXbswA8pwkBBwoBdw&S=AQAAAilXTMo9H0FUKq10tebhtHE; PRF=t%3DQQQ%252BAPI%252BUVXY; _chartbeat5=' \
  -H 'priority: u=0, i' \
  -H 'referer: https://finance.yahoo.com/quote/QQQ/history/' \
  -H 'sec-ch-ua: "Not/A)Brand";v="8", "Chromium";v="126", "Google Chrome";v="126"' \
  -H 'sec-ch-ua-mobile: ?0' \
  -H 'sec-ch-ua-platform: "macOS"' \
  -H 'sec-fetch-dest: document' \
  -H 'sec-fetch-mode: navigate' \
  -H 'sec-fetch-site: same-site' \
  -H 'sec-fetch-user: ?1' \
  -H 'upgrade-insecure-requests: 1' \
  -H 'user-agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36')
      
insertSql="INSERT INTO daily_pricing_data (date, open, high, low, close, adjusted_close, volume, product) VALUES"

for i in $response
do
  :
  if [[ "$i" != "Date,Open,High,Low,Close,Adj" ]] && [[ "$i" != "Close,Volume" ]];
  then
    fields=()
    for k in $(echo $i | tr "," "\n")
    do
      :
      fields+=("$k")
    done
    insertSql+="('${fields[0]}', ${fields[1]}, ${fields[2]}, ${fields[3]}, ${fields[4]}, ${fields[5]}, ${fields[6]}, '$symbol'),"
  fi
done

> insert.sql
echo ${insertSql%?} >> insert.sql
                  