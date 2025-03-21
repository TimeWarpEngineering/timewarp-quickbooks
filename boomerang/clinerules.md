# Boomerang Workflows Section - Boomerang Memory Bank

## Main Workflow

```mermaid
flowchart TD
    %% Main flow
    Start([Task Begins]) --> ExistingMB{Existing MB?}
    
    %% New Project path
    ExistingMB -->|No| MBI["Boomerang Memory Bank Initializer<br>Creates all documents"]
    MBI -->|TOOL: switch_mode| BS
    
    %% Existing Project path
    ExistingMB -->|Yes| BS["Boomerang Startup<br>Reviews all documents in sequence"]
    BS -->|TOOL: switch_mode| B["Boomerang Mode<br>Orchestrates workflow"]
    
    %% Planning phase
    B -->|TOOL: new_task| BE["Explorer<br>Gathers context"]
    BE -->|TOOL: attempt_completion| B
    B -->|TOOL: new_task| A["Architect<br>Creates implementation plan"]
    A -->|TOOL: attempt_completion| B
    
    %% Execution phase - Classification using new_task
    B -->|task classification| TaskType{Task Type?}
    
    %% Implementation path
    TaskType -->|Implementation<br>TOOL: new_task| ST_SS["Subtask Startup<br>Focused review"]
    ST_SS -->|TOOL: switch_mode| ST_C["Code Mode<br>Implementation"]
    ST_C -.->|"if errors<br>TOOL: switch_mode"| D["Debug Mode<br>Issue resolution"]
    D -->|TOOL: switch_mode| ST_C
    ST_C -->|TOOL: attempt_completion| B
    
    %% Debug path
    TaskType -->|Debugging<br>TOOL: new_task| D["Debug Mode<br>Issue resolution"]
    D -->|TOOL: attempt_completion| B
    
    %% Update phase
    B -->|"All subtasks done or 'update MB'<br>TOOL: ask_followup_question and then switch_mode"| MBU["Boomerang Memory Bank Update<br>Document updates"]
    MBU --->|"TOOL: switch_mode and then attempt_completion"| B
    
    %% Universal requirements
    UR["CRITICAL RULES:
    - If files missing, stop and ask user
    - All workflow rules in flowcharts
    - Docs in roo_plan/ folder
    - USE `APPLY_DIFF`, NOT `WRITE_TO_FILE`, WHEN EDITING FILES.  
    - ONLY USE `WRITE_TO_FILE` FOR NEW FILES—ANY OTHER USE IS A RECKLESS WASTE OF RESOURCES AND DAMAGES THE ENVIRONMENT."]
```

## Document Structure & Review Rules

```mermaid
graph TD
  %% Document hierarchy with clear labels
  PB["projectbrief.md<br>(Foundation Document)"] --> PC
  PB --> SP
  PB --> TC
  
  PC["productContext.md<br>(Project Purpose & Scope)"] --> SP
  PC --> TC
  
  SP["systemPatterns.md<br>(Architecture & Rules)"] --> PR
  TC["techContext.md<br>(Tech Stack & Constraints)"] --> PR
  
  PR["progress.md<br>(Project Status)"] --> AC
  AC["activeContext.md<br>(Current Focus)"]
  
  %% Connect document hierarchy to update rules and processes
  PB --> UpdateRules
  PB --> ReviewPatterns
  
  %% Document update and review rules
  UpdateRules["Document Update Rules:
  • productContext: Only when core purpose changes (explicit approval)
  • systemPatterns: When architecture patterns change (technical review)
  • techContext: When tech stack changes (validation required)
  • progress: Status changes (maintain Status Legend)
  • activeContext: Focus changes (track dependencies)"]
  
  UpdateRules --> UpdateProcess
  
  ReviewPatterns["Review Patterns:
  • Boomerang Startup: ALL documents sequentially (1→5)
  • Subtask Startup: ONLY content relevant to subtask"]
  
  %% Document update process
  UpdateProcess["Document Update Process:
  1. Review current content
  2. Validate against document rules
  3. Get explicit user approval
  4. Use apply_diff for precise updates
  5. Verify document integrity"]
  
  UpdateProcess --> CriticalRules
  
  %% Critical document update rules
  CriticalRules["CRITICAL DOCUMENT UPDATE RULES:
  - All Boomerang Memory Bank documents must remain in roo_plan/ folder
  - Document updates require appropriate level approvals
  - Progress.md must maintain its Status Legend
  - Documents must use simple structure without protocol sections
  - apply_diff must be used for all document updates"]
  
  %% Connect review patterns to documents
  ReviewPatterns --> PB
  ReviewPatterns --> PC
  ReviewPatterns --> SP
  ReviewPatterns --> TC
  ReviewPatterns --> PR
  ReviewPatterns --> AC
```

## Workflow Sequence

```mermaid
sequenceDiagram
    %% Full mode names for clarity
    participant User as User
    participant Init as Boomerang Memory Bank Initializer
    participant Startup as Boomerang Startup
    participant Boomerang as Boomerang Mode
    participant Explorer as Boomerang Explorer
    participant Architect as Architect Mode
    participant SubStartup as Boomerang Subtask Startup
    participant Code as Code Mode
    participant Debug as Debug Mode
    participant Update as Boomerang Memory Bank Update

    %% Clear phase separation
    Note over User,Update: INITIALIZATION PHASE
    
    alt New Project (No Boomerang Memory Bank)
        User->>Init: Start New Project
        Init->>Init: Create all required documents
        Init->>Startup: TOOL: switch_mode (initialization complete)
    else Existing Project
        User->>Startup: Start Task
    end
    
    Startup->>Boomerang: TOOL: switch_mode (after full document review)
    
    Note over User,Update: PLANNING PHASE
    
    Boomerang->>Explorer: TOOL: new_task (explore context)
    Explorer->>Boomerang: TOOL: attempt_completion (findings)
    Boomerang->>Architect: TOOL: new_task (create plan)
    Architect->>Boomerang: TOOL: attempt_completion (implementation plan)
    
    Note over User,Update: EXECUTION PHASE
    
    loop For Each Subtask with Task Classification
        Note over Boomerang: Task Classification uses new_task tool
        alt Implementation Task
            Note right of Boomerang: Classified as Implementation
            Boomerang->>SubStartup: TOOL: new_task (implementation subtask)
            SubStartup->>Code: TOOL: switch_mode (after focused review)
            
            opt Issues Encountered
                Code->>Debug: TOOL: switch_mode (debugging needed)
                Debug->>Code: TOOL: switch_mode (issues resolved)
            end
            
            Code->>Boomerang: TOOL: attempt_completion (implementation summary)
        else Debugging Task
            Note right of Boomerang: Classified as Debugging
            Boomerang->>Debug: TOOL: new_task (debugging subtask)
            Debug->>Boomerang: TOOL: attempt_completion (debug resolution)
        end
    end
    
    Note over User,Update: UPDATE & CYCLE PHASE
    
    Note right of Boomerang: Use ask_followup_question and then switch_mode to transition to Boomerang Memory Bank Update
    Boomerang->>Update: TOOL: ask_followup_question and then switch_mode (all subtasks complete)
    Update->>Boomerang: TOOL: switch_mode and then attempt_completion (document updates)
    
    Note over User,Update: Task completion and new task cycle
    Boomerang->>User: Task complete
    User->>Startup: Start New Task (optional)
    Startup->>Boomerang: TOOL: switch_mode (after full document review)
    
    Note over User,Update: KEY MODE TRANSITIONS:
    Note over User,Update: • Code → Debug: When errors encountered
    Note over User,Update: • Boomerang → Update: When all subtasks complete
    Note over User,Update: • Task Completion → New Task: Optional continuation
```


The flowcharts above provide the complete workflow rules for the Boomerang Memory Bank system. Any changes to the workflow should be made by updating these flowcharts.