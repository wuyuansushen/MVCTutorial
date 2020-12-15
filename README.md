# MVCTutorial

## 1.Install dotnet 5.0

## 2.Configure Apache(httpd) to reverse proxy

```
<LocationMatch "/net/">
ProxyPreserveHost On
ProxyPass "http://127.0.0.1:5000/"
ProxyPassReverse "http://127.0.0.1:5000/"
</LocationMatch>
```

## 3.Configure systemd service

Add service file `/etc/systemd/system/aspnetcoreWeb.service`.

```
[Unit]
Description=.NET Web App running on CentOS 8

[Service]
WorkingDirectory=/var/www/publish
ExecStart=/usr/bin/dotnet /var/www/publish/MVCTutorial.dll
KillSignal=SIGINT
SyslogIdentifier=dotnet-ALL
User=apache
Environment=ASPNETCORE_ENVIRONMENT=Production 

[Install]
WantedBy=multi-user.target
```

> :zany_face: Service file `/etc/systemd/system/aspnetcoreWeb.service` need **r-x** premission.

## 4. (Optional) Configure SELinux

Install `semanage`
```
dnf -y install policycoreutils-python-utils
```

Change the Diretory and its Contents SELinux Types

```
semanage fcontext -a -t httpd_sys_content_t "/var/www/publish(/.*)?"
```

Apply new Contents SELinux Types
```
restorecon -R -v /var/www/publish
```

> :zany_face: `/var/www/publish` need **r-x** premission for user **apache** at least.

## 5.Troubleshoot

### 5.1 Framework version
Check Server Framework version

```
dotnet --info
```

Check framework version of published project in `<application>.runtimeconfig.json`file under published directory

>:warning: Modify value of `runtimeOptions.framework.version` from `"5.0.0"` to `"5.0.0-preview.8.20414.8"`( shown by `dotnet --info` )
```
{
  "runtimeOptions": {
    "tfm": "net5.0",
    "framework": {
      "name": "Microsoft.AspNetCore.App",
      "version": "5.0.0"
    },
    "configProperties": {
      "System.GC.Server": true,
      "System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization": false
    }
  }
}
```

### 5.2 Package Content Directory together
Package all Directories under project exclude these three directories( `bin`, `obj`, `Properties` )

```
cp -r <Directories> <ProjectLocation>\bin\Release\net5.0\publish\
```

### 5.3 SELinux Configuration
Allow Apache proxy/proxyreverse function to Kestrel web server( Implemented in ASP.NET Core )

```
setsebool -P httpd_can_network_connect 1
```
