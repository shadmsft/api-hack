using models;

namespace models
{
    public class CustomerSales
    {
        public bool CustomerAccessDenied { get; set; }
        public bool SalesOrderHeaderDenied { get; set; }
        public bool ProductDenied { get; set; }
        public List<Customer> customers { get; set; }
        public List<SalesOrderHeader> salesOrderHeaders { get; set; }   
        public List<Product> products { get; set; }
    }
}
