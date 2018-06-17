pipeline {
  agent any
  stages {
    stage('Restore') {
      parallel {
        stage('Restore') {
          steps {
            sh 'dotnet clean'
            sh 'dotnet restore'
            dir(path: './Client/') {
              sh 'dotnet restore'
            }

            dir(path: './Server/') {
              sh 'dotnet restore'
            }

          }
        }
        stage('Update Electron Packager') {
          steps {
            sh 'sudo npm install electron-packager -g'
          }
        }
      }
    }
    stage('Build') {
      parallel {
        stage('Build Server') {
          steps {
            dir(path: './Server/') {
              sh 'dotnet publish -c Release -o ../Build/Server/'
            }

          }
        }
        stage('Build Client Win') {
          steps {
            dir(path: './Client/') {
              sh 'dotnet electronize build /target win -f netcoreapp2.0 -o ../Build/Client/win'
            }

          }
        }
      }
    }
    stage('Build Linux') {
      steps {
        dir(path: './Client/') {
          sh 'dotnet electronize build /target linux -f netcoreapp2.0 -o ../Build/Client/linux'
        }

      }
    }
    stage('Zip') {
      parallel {
        stage('Zip Clients') {
          steps {
            sh 'zip ./Build/win.zip -r ./Client/bin/desktop/ElectronNET.Host-win32-x64'
            sh 'zip ./Build/linux.zip -r ./Client/bin/desktop/ElectronNET.Host-linux-x64'
          }
        }
        stage('Zip Server') {
          steps {
            sh 'zip ./Builds/Server.zip -r ./Builds/Server/'
          }
        }
      }
    }
    stage('Add Artifacts') {
      steps {
        archiveArtifacts(artifacts: 'Build/*.zip', onlyIfSuccessful: true)
      }
    }
  }
}