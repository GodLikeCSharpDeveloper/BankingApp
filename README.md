## Quick Start

1. Install Docker: https://www.docker.com/products/docker-desktop

2. Open the solution in Visual Studio.

3. In Solution Explorer, right-click on `docker-compose` → **Set as Startup Project**.

4. Press **F5** or **Start Debugging** to launch the entire environment using Docker.

The application will be available at **http://localhost:8085**.  
If you need to change the port, update the `ports:` section in `docker-compose.yml`:

```yaml
ports:
  - "8085:8080"  # Change 8085 to another external port if needed
