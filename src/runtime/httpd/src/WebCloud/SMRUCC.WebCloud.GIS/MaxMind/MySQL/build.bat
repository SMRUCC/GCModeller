@echo off

Reflector --reflects /sql ./geolite2.sql /namespace MaxMind.geolite2 /split --language visualbasic 
Reflector /MySQL.Markdown /sql ./geolite2.sql > "./geolite2-dev-docs.md"