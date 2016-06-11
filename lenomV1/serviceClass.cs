namespace lenomV1
{
    //this class contains all the fields that are used to deserialize the json response
    class serviceClass
    {
            public int id { get; set; }
            public string img_sources { get; set; }
            public string categorie { get; set; }
            public string commune { get; set; }
            public string company_name { get; set; }
            public string wilaya { get; set; }
            public string geo_x { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string geo_y { get; set; }
            public string tel { get; set; }
            public string password { get; set; }
            public string my_tel { get; set; }
    }
}
