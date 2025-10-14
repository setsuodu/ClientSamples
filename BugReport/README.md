# TODO:

## 一、安装数据库
客户端捕捉异常，以json格式提交，MySQL对json处理比较吃力，这里选用postgreSQL。

下载镜像

```
docker pull postgres

```

运行容器
```
docker run -d --name some-postgres -e POSTGRES_PASSWORD=mysecretpassword -p 5432:5432 postgres
```

检查容器是否开启，并正常运行
```
docker ps
```

## 二、asp.netcore连接数据库
Nuget 查找包 Npgsql.EntityFrameworkCore.PostgreSQL，安装。
appsettings.json中配置数据库访问参数
```
{
	"ConnectionStrings": {
		"DefaultConnection": "Host=localhost;Port=5432;Database=YourDatabaseName;Username=YourUsername;Password=YourPassword"
	}
}
```

DbContext连接数据库
```
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

## 三、数据库界面工具
pgAdmin
Navicat for PostgreSQL

## 四、网页以列表显示bug