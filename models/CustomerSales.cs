using models;

namespace models
{
    public class CustomerSales
    {
        public List<Customer> customers { get; set; }
        public List<SalesOrderHeader> salesOrderHeaders { get; set; }   
        public List<Product> products { get; set; }
    }
}
