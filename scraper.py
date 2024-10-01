from dataclasses import dataclass, field
import uuid
import requests
from bs4 import BeautifulSoup, ResultSet, Tag
from typing import List
import csv
import boto3
import os

aws_access_key_id = os.environ.get('AWS_ACCESS_KEY_ID')
aws_secret_access_key = os.environ.get('AWC_SECRET_ACCESS_KEY')
region_name = os.environ.get('REGION_NAME')


@dataclass
class Opdracht:
    title: str
    locatie: str
    uren: str
    duur: str
    link: str


@dataclass
class Opdrachten:
    dataList: List[Opdracht] = field(default_factory=list)
    
zoektermen = [
    'tester', 'ict', 'scrum+master', 'agile+coach',
    'functioneel beheerder','project manager', 'project leider',
    'automatisch testen'
    ]   

links = set()


def getOpdrachten(opdrachten : ResultSet[Tag], opdrachten_lijst: Opdrachten):
    for opdracht in opdrachten:
        link = opdracht.get('href')
        if link in links:
            break
        links.add(link)
        
        title = opdracht.select_one('.entry-title').get_text(strip=True)
        locatie = opdracht.select_one('.u-location-icon').get_text(strip=True)
        uren = opdracht.select_one('.u-hours-icon').get_text(strip=True)
        duur = opdracht.select_one('.u-date-icon').get_text(strip=True)
        
        opdrachten_lijst.dataList.append(Opdracht(title=title, locatie=locatie, uren=uren, duur=duur, link=link))

def writeToDynamoDB(opdrachten_lijst: Opdrachten):
    dynamodb = boto3.resource(
    'dynamodb',
    aws_access_key_id=aws_access_key_id,
    aws_secret_access_key=aws_secret_access_key,
    region_name=region_name
    )
    table = dynamodb.Table('ICTRebelsOpdrachten')
    for opdracht in opdrachten_lijst.dataList:
        # Generate a unique UUID for each item
        item_uuid = str(uuid.uuid4())
        
        # Write the item to DynamoDB
        table.put_item(
            Item={
                'UUID': item_uuid,
                'Broker': 'BlueTrail',
                'Titel': opdracht.title,
                'Locatie': opdracht.locatie,
                'Uren': opdracht.uren,
                'Duur': opdracht.duur,
                'Link': opdracht.link
            }
        )
# def writeCSV(opdrachten_lijst: Opdrachten):
#     fileName = "output.csv"
#     header = ["Titel", "Locatie", "Uren", "Duur", "Link"]
#     with open(fileName, mode="w", newline='',) as file:
#         writer = csv.writer(file, delimiter=';')
#         writer.writerow(header)
#         for opdracht in opdrachten_lijst.dataList:
#             writer.writerow([opdracht.title, opdracht.locatie, opdracht.uren, opdracht.duur, opdracht.link])
    


if __name__ == "__main__":
    opdrachten_lijst = Opdrachten()
    for zoekterm in zoektermen:
        response = requests.get("https://www.bluetrail.nl/opdrachten/?s=" + zoekterm)
        page_content = response.text
        soup = BeautifulSoup(page_content, 'html.parser')
        last_page = soup.select('.page-numbers')[-2].get_text() if soup.select('.page-numbers') else 1
        for i in range(int(last_page) ):
            url = f'https://www.bluetrail.nl/opdrachten/page/{i + 1}/?s={zoekterm}'
            response = requests.get(url)
            page_content = response.text
            soup = BeautifulSoup(page_content, 'html.parser')
            opdrachten = soup.select('.ysdms-main>a')
            getOpdrachten(opdrachten, opdrachten_lijst)
    writeToDynamoDB(opdrachten_lijst)
    # writeCSV(opdrachten_lijst)
        