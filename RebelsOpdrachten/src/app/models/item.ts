export type ItemResponse = {
    totalCount: number;
    items : Item[];
    lastEvaluatedKey: string;
}

export type Item = {
    uuid: string;
    titel: string;
    broker: string;
    locatie: string;
    uren: string;
    duur: string;
    link: string
}