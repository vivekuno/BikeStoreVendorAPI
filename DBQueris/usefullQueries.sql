SELECT  
      s.[product_id],p.product_name
      ,sum(s.[quantity]) as totalQuantity
  FROM [BikeStores].[production].[stocks] s join [production].products p on s.product_id = p.product_id
  join production.categories c on p.category_id = c.category_id 
  where c.category_name = 'Mountain Bikes'
  group by s.[product_id],p.product_name
  

  SELECT  
      p.product_name ProductName
      ,sum(s.[quantity]) as totalQuantity
  FROM [BikeStores].[production].[stocks] s join [production].products p on s.product_id = p.product_id
  join production.categories c on p.category_id = c.category_id 
  where c.category_name = 'Mountain Bikes'
  group by p.product_name

  update sales.orders set order_date = DATEADD(YEAR,-1,order_date), required_date = DATEADD(YEAR,-1,required_date), shipped_date = DATEADD(YEAR,-1,shipped_date)
