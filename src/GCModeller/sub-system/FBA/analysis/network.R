# 加载必要的包
library(igraph)
library(ggplot2)

# 创建代谢网络图
create_metabolic_network <- function(edges, nodes, output_file = "metabolic_network.png") {
  # 创建图对象
  g <- graph_from_data_frame(d = edges, directed = TRUE)
  
  # 设置节点属性
  V(g)$size <- degree(g) * 5
  V(g)$color <- ifelse(degree(g) > 3, "red", "lightblue")
  
  # 设置边属性
  E(g)$width <- edges$flux / max(edges$flux) * 3
  E(g)$color <- ifelse(edges$flux > 10, "red", "gray")
  
  # 设置布局
  layout <- layout_with_fr(g)
  
  # 绘制网络图
  plot(g,
     vertex.label = V(g)$label,
     vertex.size = V(g)$size,
     vertex.color = V(g)$color,
     edge.width = E(g)$width,
     edge.color = E(g)$color,
     layout = layout,
     main = "代谢网络图",
     vertex.label.cex = 0.8,
     vertex.label.color = "black"
  )
  
  # 保存图像
  dev.copy(png, output_file, width = 10, height = 8)
  dev.off()
}

# 使用示例数据
example_edges <- data.frame(
  from = c("M1", "M2", "M3", "M4", "M5", "M6"),
  to = c("M2", "M3", "M1", "M5", "M6", "M4"),
  flux = c(15.2, 8.7, 12.3, 3.5, 20.1,99)
)

example_nodes <- data.frame(
  id = c("M1", "M2", "M3", "M4", "M5", "M6"),
  label = c("葡萄糖-6-磷酸", "果糖-6-磷酸", "丙酮酸", "琥珀酸", "柠檬酸", "异柠檬酸"),
  x = c(1, 2, 3, 4, 5, 6),
  y = c(1, 2, 3, 4, 5, 6)
)

# 生成网络图
create_metabolic_network(example_edges, example_nodes)