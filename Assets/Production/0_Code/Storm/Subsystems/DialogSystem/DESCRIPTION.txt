namespace Storm.DialogSystem {
  Code used to make game dialogs.

  The Dialog system of the game is comprised of a graph system.
  
  Every dialog GRAPH is comprised of a series of dialog NODES. 
    - Every dialog NODE has an ordered list of SNIPPETs. 
      - Each SNIPPET is one "window" of text.
    - Every dialog NODE also has a list of decisions the player can make.
      - Decisions are essentially the "EDGES" or "TRANSITIONS" of the graph.
      - Every decision has a list of snippets that are the "consequences" that play out before
        transitioning to the next node in the graph. This allows you to create situations where an NPC
        reacts to two different decisions slightly differently, but still transition to the same node.
        
  Each conversation is it's own graph (i.e., NPCs don't share graphs).
}