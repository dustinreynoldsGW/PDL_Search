const testdata = [
    {
        DrugName : 'NEXIUM DR 10 MG PACKET<br />ESOMEPRAZOLE MAGNESIUM',
        NDC : '00186401001',
        DrugClass: 'GASTROINTESTINAL : Proton Pump Inhibitors',
        RxOTC : 'Rx',
        PDLStatus: 'Preferred PDL Agent'
    },
    {
        DrugName: 'NEXIUM DR 2.5 MG PACKET<br />ESOMEPRAZOLE MAGNESIUM',
        NDC: '00186402501',
        DrugClass: 'GASTROINTESTINAL : Proton Pump Inhibitors',
        RxOTC: 'Rx',
        PDLStatus: 'Preferred PDL Agent'
    },
    {
        DrugName: 'NEXIUM DR 20 MG PACKET<br />ESOMEPRAZOLE MAGNESIUM',
        NDC: '00186402031',
        DrugClass: 'GASTROINTESTINAL : Proton Pump Inhibitors',
        RxOTC: 'Rx',
        PDLStatus: 'Non-Preferred PDL Agent'
    }
];

const columns = {
    DrugName: 'Drug Name & Stength / Generic Name',
    NDC: 'NDC',
    DrugClass: 'Class',
    RxOTC: 'RX/OTC',
    PDLStatus: 'PDL Status'
};

//export { data, columns };
