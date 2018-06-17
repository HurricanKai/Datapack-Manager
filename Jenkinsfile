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
    stage('Build Server') {
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
    stage('Pack Client') {
      parallel {
        stage('Pack Server') {
          steps {
            dir(path: './Build/') {
              archiveArtifacts(artifacts: './Server', onlyIfSuccessful: true)
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
      }
    }
    stage('Pack Clients') {
      steps {
        dir(path: './Client/bin/desktop/') {
          archiveArtifacts(artifacts: './win', onlyIfSuccessful: true)
          archiveArtifacts(artifacts: './linux', onlyIfSuccessful: true)
        }

      }
    }
  }
}