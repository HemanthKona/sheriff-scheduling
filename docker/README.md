# Running the Application on Docker

## Management Script

The `manage` script wraps the Docker process in easy to use commands.

To get full usage information on the script, run:

```
./manage -h
```

## Building the Images

The first thing you'll need to do is build the Docker images.

To build the images, run:

```
./manage build
```

## Restore a backup of the database

Create a `/c/sheriff-scheduling/docker/tmp` folder if it does not exist.
Place a backup archive in that folder.

### Using the `restore` command (Recommended)

This is the recommended approach to restore a backup in order to initialize/reset the API. The `restore` command automates the process of resetting the API's database and restoring a fresh copy from backup, allowing you to easily initialize or reset the API during development and testing.

```
Wade@Epoch MINGW64 /c/sheriff-scheduling/docker (master)
$ ./manage restore postgres-appdb_2020-03-06_13-42-56.sql.gz
```

## Starting the Project

To start the project, run:

```
./manage start
```

This will start the project interactively; with all of the logs being written to the command line. Press `Ctrl-C` to shut down the services from the same shell window.

Any environment variables containing settings, configuration, or secrets can be placed in a `.env` file in the `docker` folder and they will automatically be picked up and loaded by the `./manage` script when you start the application.

## Stopping the Project

To stop the project, run:

```
./manage stop
```

This will shut down and clean up all of the containers in the project. This is a non-destructive process. The containers are not deleted so they will be reused the next time you run start.

Since the services are started interactively, you will have to issue this command from another shell window. This command can also be run after shutting down the services using the `Ctrl-C` method to clean up any services that may not have shutdown correctly.

## Access to the Application

The database will need your user ID in the "User" table. Access to the Docker DB can be done using pgAdmin or similar tool. The user ID is the IDIR you are using. In addition, a row in "UserRole" is required. The SQL to do this would be something similar to this:

```
INSERT INTO public."User"(
	"Id", "IdirName", "IdirId", "KeyCloakId", "IsEnabled", "FirstName", "LastName", "Email", "Discriminator", "HomeLocationId")
	VALUES ('00000000-0000-0000-0000-000000000002', 'REPLACE_WITH_YOUR_IDIR_USERNAME', 'REPLACE_WITH_YOUR_IDIR_USER_GUID', 'REPLACE_WITH_YOUR_IDIR_USER_GUID', True, 'REPLACE_WITH_YOUR_FIRSTNAME', 'REPLACE_WITH_YOUR_LASTNAME', 'REPLACE_WITH_YOUR_EMAIL@gov.bc.ca', 'Sheriff', 6);
insert into "UserRole" ("UserId", "RoleId", "EffectiveDate")
values ('00000000-0000-0000-0000-000000000002', 1, '2024-01-01');
```

One last piece is to update the record in the "User" table so that the IdirId value is the proper uuid. This can be obtained by running the application locally as below, then decoding the token (use https://jwt.io/) from the Bearer token. You can get the proper value for the IdirId in `idir_user_guid` property in the decoded jwt and update the User table appropriately.

## Using the Application

- The main UI is exposed at; http://localhost:8080/
- The API is available at; http://localhost:8080/api
  - The API is also exposed directly at; http://localhost:5000/api
