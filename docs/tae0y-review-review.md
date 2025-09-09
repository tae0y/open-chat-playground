# `@tae0y` REVIEW REVIEW

1. 특정 PR의 리뷰 코멘트 조회

    ```
    gh api graphql -f query='{ 
      repository(owner: "aliencube", name: "open-chat-playground") {
        pullRequest(number: 409) {
          reviewThreads(first: 100) {
            nodes {
              comments(first: 100) {
                nodes {
                  author { login }
                  body
                  createdAt
                  path
                  position
                }
              }
            }
          }
        }
      }
    }' > ./409_timeline.json
    ```