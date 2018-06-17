pipeline {
  agent any
  stages {
    stage('Restore') {
      parallel {
        stage('Restore') {
          steps {
            sh 'dotnet restore'
            dir(path: './Client/') {
              sh 'dotnet clean'
              sh 'dotnet restore'
            }

            dir(path: './Server/') {
              sh 'dotnet clean'
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
        stage('Build Client Linux') {
          steps {
            dir(path: './Client/') {
              sh 'dotnet electronize build /target linux -f netcoreapp2.0 -o ../Build/Client/linux'
            }

          }
        }
        stage('Build Client OSX') {
          steps {
            dir(path: './Client/') {
              sh '''
    
    
    
    dotnet electronize build /target osx -f netcoreapp2.0 -o ../Build/Client/osx'''
            }

          }
        }
      }
    }
    stage('Pack Client') {
      parallel {
        stage('Pack Client') {
          steps {
            dir(path: './Client/bin/desktop/') {
              archiveArtifacts(artifacts: './win', onlyIfSuccessful: true)
              archiveArtifacts(artifacts: './osx', onlyIfSuccessful: true)
              archiveArtifacts(artifacts: './linux', onlyIfSuccessful: true)
            }

          }
        }
        stage('Pack Server') {
          steps {
            dir(path: './Build/') {
              archiveArtifacts(artifacts: './Server', onlyIfSuccessful: true)
            }

          }
        }
      }
    }
  }
}